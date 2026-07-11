using MediSync.Web.Services.ServiceResponse;
using System.Text;
using System.Text.Json;

namespace MediSync.Web.Services;

public class AuthService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public AuthService(IHttpClientFactory httpClientFactory)
    {
        this._httpClientFactory = httpClientFactory;
    }

    public async Task<ServiceResponseMessage<Guid>> RegisterAsync(string firstName, string lastName, string email, string password, string role)
    {
        var payLoad = new
        {
            firstName,
            lastName,
            email,
            password,
            role
        };

        var json = JsonSerializer.Serialize(payLoad);
        StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClientFactory.CreateClient().PostAsync("https://localhost:7000/auth/register", content);

        if (!response.IsSuccessStatusCode)
        {
            var resultError = await response.Content.ReadFromJsonAsync<ServiceResponseMessage<Guid>>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return ServiceResponseMessage<Guid>.Failure(
                resultError!.Error.Code ?? "",
                resultError.Error.Message ?? "",
                resultError.Error.Type ?? "Failure"
            );
        }

        var resultSuccess = await response.Content.ReadFromJsonAsync<ServiceResponseMessage<Guid>>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return ServiceResponseMessage<Guid>.Success(resultSuccess!.Value);
    }

    public async Task<ServiceResponseMessage<LoginResponse>> LoginAsync(string email, string password)
    {
        var payLoad = new
        {
            email,
            password,
        };

        var json = JsonSerializer.Serialize(payLoad);
        StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClientFactory.CreateClient().PostAsync("https://localhost:7000/auth/login", content);

        if (!response.IsSuccessStatusCode)
        {
            return ServiceResponseMessage<LoginResponse>.Failure(response.StatusCode.ToString(), "Login failed", "Failure");
        }

        var result = await response.Content.ReadFromJsonAsync<ServiceResponseMessage<LoginResponse>>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return result!;
    }
}

public record LoginResponse(
    string Token,
    Guid UserId,
    string FirstName,
    string LastName,
    string Email,
    string Role
);