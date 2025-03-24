using Serilog;

namespace TypingMaster.Business;

public class MockIdpService
{
    private readonly ILogger _logger;

    public MockIdpService(ILogger logger)
    {
        _logger = logger;
    }

    public async Task<IdpAuthResponse> AuthenticateAsync(string email, string password)
    {
        // Mock successful authentication for development
        // In production, this would be replaced with actual IDP calls
        await Task.Delay(100); // Simulate network delay

        return new IdpAuthResponse
        {
            Success = true,
            AccessToken = $"mock.jwt.token.{Guid.NewGuid()}",
            TokenType = "Bearer",
            ExpiresIn = 3600,
            RefreshToken = $"mock.refresh.token.{Guid.NewGuid()}",
            Scope = "openid profile email"
        };
    }

    public async Task<IdpAuthResponse> RefreshTokenAsync(string refreshToken)
    {
        // Mock token refresh
        await Task.Delay(100);

        return new IdpAuthResponse
        {
            Success = true,
            AccessToken = $"mock.jwt.token.{Guid.NewGuid()}",
            TokenType = "Bearer",
            ExpiresIn = 3600,
            RefreshToken = refreshToken,
            Scope = "openid profile email"
        };
    }
}

public class IdpAuthResponse
{
    public bool Success { get; set; }
    public string AccessToken { get; set; } = string.Empty;
    public string TokenType { get; set; } = string.Empty;
    public int ExpiresIn { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
    public string Scope { get; set; } = string.Empty;
}