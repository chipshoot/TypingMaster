using Serilog;
using System.Text;
using System.Text.Json;
using TypingMaster.Business.Contract;

namespace TypingMaster.Business;

public class AuthService : ServiceBase, IAuthService
{
    private readonly HttpClient _httpClient;

    public AuthService(HttpClient httpClient, ILogger logger) : base(logger)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> LoginAsync(string userName, string password)
    {
        var token = await AuthenticateWithIdpAsync(userName, password);
        if (!string.IsNullOrEmpty(token))
        {
            // Store the token (e.g., in local storage or a secure cookie)
            // For demonstration purposes, we'll just return true
            return true;
        }
        return false;
    }

    public Task LogoutAsync()
    {
        // Implement logout logic (e.g., clear the stored token)
        return Task.CompletedTask;
    }

    public Task<bool> RegisterAsync(string userName, string password)
    {
        // Implement user registration logic
        return Task.FromResult(true);
    }

    public async Task<string> AuthenticateWithIdpAsync(string userName, string password)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "https://your-idp-url.com/token");
        var content = new StringContent(JsonSerializer.Serialize(new
        {
            client_id = "your-client-id",
            client_secret = "your-client-secret",
            grant_type = "password",
            username = userName,
            password
        }), Encoding.UTF8, "application/json");

        request.Content = content;
        var response = await _httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent);
            return tokenResponse?.AccessToken;
        }

        return null;
    }

    private class TokenResponse
    {
        public string AccessToken { get; set; }
    }
}