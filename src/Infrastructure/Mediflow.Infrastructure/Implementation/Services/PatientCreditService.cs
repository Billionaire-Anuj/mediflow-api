using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Security.Cryptography;
using Mediflow.Domain.Common;
using Mediflow.Domain.Entities;
using Mediflow.Domain.Common.Enum;
using Mediflow.Application.Exceptions;
using Mediflow.Application.DTOs.Payments;
using Mediflow.Application.Common.User;
using Mediflow.Application.Interfaces.Data;
using Mediflow.Application.Interfaces.Services;
using Mediflow.Application.Settings;
using Mediflow.Domain.Common.Enum.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Mediflow.Infrastructure.Implementation.Services;

public class PatientCreditService(
    IApplicationDbContext applicationDbContext,
    IApplicationUserService applicationUserService,
    IUserPropertyService userPropertyService,
    INotificationService notificationService,
    IHttpClientFactory httpClientFactory,
    IOptions<KhaltiSettings> khaltiOptions,
    IOptions<EsewaSettings> esewaOptions) : IPatientCreditService
{
    private const string ProviderKhalti = "KHALTI";
    private const string ProviderEsewa = "ESEWA";

    public async Task<KhaltiInitiateResponseDto> InitiateKhaltiTopupAsync(CreditTopupRequestDto request)
    {
        var amount = NormalizeAmount(request.Amount);
        var patient = GetPatientUser();
        var settings = khaltiOptions.Value;

        var orderId = $"CREDIT-{Guid.NewGuid():N}";
        var payload = new
        {
            return_url = settings.ReturnUrl,
            website_url = settings.WebsiteUrl,
            amount = (int)Math.Round(amount * 100, 0),
            purchase_order_id = orderId,
            purchase_order_name = "Mediflow Credits",
            customer_info = new
            {
                name = patient.Name,
                email = patient.EmailAddress,
                phone = patient.PhoneNumber
            }
        };

        var response = await SendKhaltiRequestAsync<KhaltiInitiateApiResponse>(settings.InitiateUrl, payload, settings.SecretKey);

        if (response == null || string.IsNullOrWhiteSpace(response.Pidx) || string.IsNullOrWhiteSpace(response.PaymentUrl))
        {
            throw new BadRequestException("Unable to initiate Khalti payment.");
        }

        SavePendingTopup(patient.Id, ProviderKhalti, response.Pidx, amount);

        return new KhaltiInitiateResponseDto
        {
            Pidx = response.Pidx,
            PaymentUrl = response.PaymentUrl
        };
    }

    public async Task<bool> ConfirmKhaltiTopupAsync(KhaltiConfirmDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Pidx))
        {
            throw new BadRequestException("Payment reference is required.");
        }

        var patient = GetPatientUser();
        var settings = khaltiOptions.Value;

        var lookupPayload = new { pidx = request.Pidx };
        var response = await SendKhaltiRequestAsync<KhaltiLookupApiResponse>(settings.LookupUrl, lookupPayload, settings.SecretKey);

        if (response == null || !string.Equals(response.Status, "Completed", StringComparison.OrdinalIgnoreCase))
        {
            throw new BadRequestException("Khalti payment has not been completed yet.");
        }

        var amount = Math.Round(response.TotalAmount / 100m, 2);

        ApplyCredits(patient, amount, request.Pidx);

        return true;
    }

    public Task<EsewaInitiateResponseDto> InitiateEsewaTopupAsync(CreditTopupRequestDto request)
    {
        var amount = NormalizeAmount(request.Amount);
        var patient = GetPatientUser();
        var settings = esewaOptions.Value;

        var transactionUuid = Guid.NewGuid().ToString("N");
        var totalAmount = amount.ToString("0.00");
        var signedFields = "total_amount,transaction_uuid,product_code";
        var signaturePayload = $"total_amount={totalAmount},transaction_uuid={transactionUuid},product_code={settings.MerchantCode}";
        var signature = ComputeHmacSignature(signaturePayload, settings.SecretKey);

        var payload = new Dictionary<string, string>
        {
            ["amount"] = totalAmount,
            ["tax_amount"] = "0",
            ["total_amount"] = totalAmount,
            ["transaction_uuid"] = transactionUuid,
            ["product_code"] = settings.MerchantCode,
            ["product_service_charge"] = "0",
            ["product_delivery_charge"] = "0",
            ["success_url"] = settings.SuccessUrl,
            ["failure_url"] = settings.FailureUrl,
            ["signed_field_names"] = signedFields,
            ["signature"] = signature
        };

        SavePendingTopup(patient.Id, ProviderEsewa, transactionUuid, amount);

        return Task.FromResult(new EsewaInitiateResponseDto
        {
            PaymentUrl = settings.InitiateUrl,
            TransactionUuid = transactionUuid,
            Payload = payload
        });
    }

    public Task<bool> ConfirmEsewaTopupAsync(EsewaConfirmDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Data))
        {
            throw new BadRequestException("Payment response data is required.");
        }

        var patient = GetPatientUser();
        var settings = esewaOptions.Value;

        var json = Encoding.UTF8.GetString(Convert.FromBase64String(request.Data));
        var response = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json)
                       ?? throw new BadRequestException("Unable to parse eSewa response.");

        if (!response.TryGetValue("status", out var statusElement) ||
            !string.Equals(statusElement.GetString(), "COMPLETE", StringComparison.OrdinalIgnoreCase))
        {
            throw new BadRequestException("eSewa payment has not been completed yet.");
        }

        var transactionUuid = GetStringField(response, "transaction_uuid");
        var productCode = GetStringField(response, "product_code");
        var totalAmount = GetStringField(response, "total_amount");
        var signedFieldNames = GetStringField(response, "signed_field_names");
        var signature = GetStringField(response, "signature");
        var transactionCode = GetStringField(response, "transaction_code");

        if (string.IsNullOrWhiteSpace(transactionCode))
        {
            transactionCode = transactionUuid;
        }

        if (string.IsNullOrWhiteSpace(transactionUuid))
        {
            throw new BadRequestException("Transaction identifier is missing.");
        }

        if (!string.Equals(productCode, settings.MerchantCode, StringComparison.OrdinalIgnoreCase))
        {
            throw new BadRequestException("Invalid merchant code.");
        }

        ValidateEsewaSignature(response, signedFieldNames, signature, settings.SecretKey);

        if (!decimal.TryParse(totalAmount, out var paidAmount))
        {
            throw new BadRequestException("Unable to read payment amount.");
        }

        ApplyCredits(patient, paidAmount, transactionCode);

        return Task.FromResult(true);
    }

    private User GetPatientUser()
    {
        var userId = applicationUserService.GetUserId;

        var patient = applicationDbContext.Users
                          .Include(x => x.Role)
                          .Include(x => x.Credit)
                          .FirstOrDefault(x => x.Id == userId)
                      ?? throw new NotFoundException("Patient not found.");

        if (patient.Role?.Id.ToString() != Constants.Roles.Patient.Id)
        {
            throw new BadRequestException("Only patients can access credits.");
        }

        return patient;
    }

    private static decimal NormalizeAmount(decimal amount)
    {
        if (amount <= 0)
        {
            throw new BadRequestException("Amount must be greater than zero.");
        }

        return Math.Round(amount, 2);
    }

    private void ApplyCredits(User patient, decimal amount, string paymentIndex)
    {
        var credit = patient.Credit;

        if (credit == null)
        {
            credit = new PatientCredit(patient.Id, 0m, string.Empty);
            applicationDbContext.PatientCredits.Add(credit);
            patient.Credit = credit;
        }

        credit.AddCredits(amount, paymentIndex);
        applicationDbContext.SaveChanges();

        notificationService.QueueNotification(
            patient.Id,
            NotificationType.System,
            "Credits added successfully",
            $"{amount:0.##} credits were added to your wallet.",
            "/patient/profile",
            $"patient-credit-topup:{paymentIndex}");

        applicationDbContext.SaveChanges();
    }

    private void SavePendingTopup(Guid userId, string provider, string paymentIndex, decimal amount)
    {
        var key = BuildTopupKey(provider, paymentIndex);

        userPropertyService.SaveProperty(userId, key, new CreditTopupConfiguration
        {
            Amount = amount,
            Provider = provider
        });
    }

    private static string BuildTopupKey(string provider, string paymentIndex)
        => $"{nameof(UserConfiguration.CREDIT_TOPUP)}_{provider}_{paymentIndex}";

    private async Task<T?> SendKhaltiRequestAsync<T>(string endpoint, object payload, string secretKey)
    {
        var client = httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("Authorization", $"Key {secretKey}");

        var json = JsonSerializer.Serialize(payload);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync(endpoint, content);

        if (!response.IsSuccessStatusCode)
        {
            throw new BadRequestException("Khalti payment request failed.");
        }

        return await response.Content.ReadFromJsonAsync<T>();
    }

    private static string ComputeHmacSignature(string payload, string secretKey)
    {
        var keyBytes = Encoding.UTF8.GetBytes(secretKey);
        var payloadBytes = Encoding.UTF8.GetBytes(payload);

        using var hmac = new HMACSHA256(keyBytes);
        var hash = hmac.ComputeHash(payloadBytes);
        return Convert.ToBase64String(hash);
    }

    private static void ValidateEsewaSignature(Dictionary<string, JsonElement> response, string signedFieldNames, string signature, string secretKey)
    {
        if (string.IsNullOrWhiteSpace(signedFieldNames) || string.IsNullOrWhiteSpace(signature))
        {
            throw new BadRequestException("Invalid eSewa signature response.");
        }

        var fields = signedFieldNames.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var payload = string.Join(",", fields.Select(field =>
        {
            if (!response.TryGetValue(field, out var value))
            {
                throw new BadRequestException($"Missing signed field '{field}'.");
            }

            return $"{field}={value.GetString() ?? value.ToString()}";
        }));

        var computedSignature = ComputeHmacSignature(payload, secretKey);

        if (!string.Equals(signature, computedSignature, StringComparison.OrdinalIgnoreCase))
        {
            throw new BadRequestException("eSewa payment signature verification failed.");
        }
    }

    private static string GetStringField(Dictionary<string, JsonElement> response, string key)
    {
        if (!response.TryGetValue(key, out var value))
        {
            return string.Empty;
        }

        return value.ValueKind switch
        {
            JsonValueKind.String => value.GetString() ?? string.Empty,
            JsonValueKind.Number => value.GetRawText(),
            _ => value.ToString()
        };
    }

    private sealed class KhaltiInitiateApiResponse
    {
        [JsonPropertyName("pidx")]
        public string Pidx { get; set; } = string.Empty;

        [JsonPropertyName("payment_url")]
        public string PaymentUrl { get; set; } = string.Empty;
    }

    private sealed class KhaltiLookupApiResponse
    {
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("total_amount")]
        public int TotalAmount { get; set; }
    }
}
