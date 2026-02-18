using System.Net.Http.Json;
using Mediflow.Application.Common.API;

namespace Mediflow.Infrastructure.Implementation.Helper;

public class ApiClientService(HttpClient httpClient) : IApiClientService
{
    public async Task<T?> GetAsync<T>(string endpoint, IDictionary<string, string>? parameters = null, IDictionary<string, string>? headersValue = null)
    {
        if (parameters is { Count: > 0 })
        {
            var queryString = string.Join("&", parameters.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));

            endpoint += "?" + queryString;
        }

        httpClient.DefaultRequestHeaders.Clear();

        if (headersValue is { Count: > 0 })
        {
            foreach (var header in headersValue)
            {
                httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
        }

        var response = await httpClient.GetAsync(endpoint);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<T>();
        }

        throw new ApplicationException($"Error: {response.StatusCode}");
    }

    public async Task<T?> PostAsync<T>(string endpoint, StringContent stringContent)
    {
        var response = await httpClient.PostAsync(endpoint, stringContent);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<T>();
        }

        throw new ApplicationException($"Error: {response.StatusCode}");
    }

    public async Task<T?> UpdateAsync<T>(string endpoint, StringContent stringContent)
    {
        var response = await httpClient.PatchAsync(endpoint, stringContent);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<T>();
        }

        throw new ApplicationException($"Error: {response.StatusCode}");
    }

    public async Task<T?> DeleteAsync<T>(string endpoint)
    {
        var response = await httpClient.DeleteAsync(endpoint);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<T>();
        }

        throw new ApplicationException($"Error: {response.StatusCode}");
    }
}