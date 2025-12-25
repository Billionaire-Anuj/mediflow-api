using System.Net;
using Grpc.Core;
using Medical.GrpcService.Entities;
using Medical.GrpcService.Entities.Enums;
using Medical.GrpcService.Token;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using Exception = System.Exception;

namespace Medical.GrpcService.Services;

public class AuthenticationGrpcService(
    UserManager<User> userManager,
    ITokenService tokenService,
    RoleManager<Role> roleManager,
    IEmailSender emailSender,
    ILogger<AuthenticationGrpcService> logger)
    : AuthenticationService.AuthenticationServiceBase
{
    private const string DOCTOR_EMAIL_DOMAIN = "medicalapp";
    private const string DOCTOR_ROLE = "Doctor";
    private const string PATIENT_ROLE = "Patient";

    public override async Task<LoginResponse> Login(LoginRequest request, ServerCallContext context)
    {
        try
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
            }

            var result = await userManager.CheckPasswordAsync(user, request.Password);
            if (!result)
            {
                throw new RpcException(new Status(StatusCode.Unauthenticated, "Invalid password"));
            }

            if (!user.IsActive || !user.EmailConfirmed)
            {
                throw new Exception("User is not active or email not confirmed yet.");
            }

            var roles = await userManager.GetRolesAsync(user);
            var token = await tokenService.CreateToken(user);

            return new LoginResponse
            {
                Token = token,
                Email = user.Email,
                Roles = { roles },
                UserId = user.Id,
            };
        }
        catch (RpcException rpcEx)
        {
            logger.LogError(rpcEx, "gRPC error for user {Email}", request.Email);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during login for user {Email}", request.Email);
            throw new RpcException(new Status(StatusCode.Internal, "An unexpected error occurred."));
        }
    }

    public override async Task<RegisterResponse> Register(RegisterRequest request, ServerCallContext context)
    {
        try
        {
            if (await userManager.FindByEmailAsync(request.Email) != null)
            {
                throw new RpcException(new Status(StatusCode.AlreadyExists, "Email already registered"));
            }

            var user = new Patient
            {
                UserName = request.Email,
                Email = request.Email,
                FullName = request.FullName ?? "Patient",
                DateOfBirth = request.DateOfBirth?.ToDateTime() ?? DateTime.UtcNow,
                Gender = Enum.TryParse<Gender>(request.Gender, true, out var gender) ? gender : Gender.Male,
                Phone = request.Phone ?? string.Empty,
                Address = request.Address ?? string.Empty,
                Created = DateTime.UtcNow,
                LastActive = DateTime.UtcNow,
                IsActive = true,
                EmailConfirmed = false
            };

            var result = await userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument,
                    string.Join(", ", result.Errors.Select(e => e.Description))));
            }

            if (!await roleManager.RoleExistsAsync(PATIENT_ROLE))
            {
                var roleResult = await roleManager.CreateAsync(new Role { Name = PATIENT_ROLE });
                if (!roleResult.Succeeded)
                {
                    logger.LogError("Failed to create role: {Role}", PATIENT_ROLE);
                    throw new RpcException(new Status(StatusCode.Internal, "Failed to create role"));
                }
            }

            var roleAssignResult = await userManager.AddToRoleAsync(user, PATIENT_ROLE);
            if (!roleAssignResult.Succeeded)
            {
                logger.LogError("Failed to assign role {Role} to user {Email}", PATIENT_ROLE, user.Email);
                throw new RpcException(new Status(StatusCode.Internal, "Failed to assign role"));
            }

            logger.LogInformation("Patient {Email} registered successfully", user.Email);

            // var token = await _tokenService.CreateToken(user);
            // var roles = await _userManager.GetRolesAsync(user);

            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebUtility.UrlEncode(token);

            var confirmationLink = $"https://localhost:7085/Account/VerifyEmail?userId={user.Id}&token={encodedToken}";
            var subject = "Verify your email";
            var body = $@"
                <h2>Welcome to Medicare, {user.FullName}!</h2>
                <p>Please verify your email by clicking the button below:</p>
                <p>
                    <a href='{confirmationLink}' style='padding: 10px 20px; background-color: #28a745; color: white; border-radius: 5px; text-decoration: none;'>Verify Email</a>
                </p>
                <p>If the button doesn't work, use this link:</p>
                <p>{confirmationLink}</p>
            ";

            await emailSender.SendEmailAsync(user.Email, subject, body);
            
            return new RegisterResponse
            {
                Success = true,
            };
        }
        catch (RpcException rpcEx)
        {
            logger.LogError(rpcEx, "gRPC error for user {Email}", request.Email);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during registration for user {Email}", request.Email);
            throw new RpcException(new Status(StatusCode.Internal, "An unexpected error occurred."));
        }
    }

    public override Task<LogoutResponse> Logout(LogoutRequest request, ServerCallContext context)
    {
        return Task.FromResult(new LogoutResponse
        {
            Success = true,
            Message = "Logged out successfully"
        });
    }
}