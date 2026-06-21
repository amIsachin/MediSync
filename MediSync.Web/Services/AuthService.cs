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

    public async Task<bool> RegisterAsync(string firstName, string lastName, string email, string password, string role)
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
        return response.IsSuccessStatusCode;
    }

    public async Task<LoginResponse> LoginAsync(string email, string password)
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
            return new LoginResponse(null, Guid.Empty, null, null, null, null);
        }

        var result = await response.Content.ReadAsStringAsync();

        var deserializeResult = JsonSerializer.Deserialize<LoginResponse>(result);

        return deserializeResult;
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