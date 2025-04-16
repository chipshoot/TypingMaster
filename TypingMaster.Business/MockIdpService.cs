using Serilog;
using TypingMaster.Business.Contract;
using TypingMaster.Core.Models;

namespace TypingMaster.Business;

public class MockIdpService : IIdpService
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

    public Task<IdpAuthResponse> RespondToNewPasswordChallengeAsync(string email, string newPassword, string session)
    {
        throw new NotImplementedException();
    }

    public Task<bool> SetPermanentPasswordAsync(string email, string password)
    {
        throw new NotImplementedException();
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

    public async Task<bool> RegisterUserAsync(RegisterRequest request)
    {
        // Mock successful registration
        await Task.Delay(100);
        _logger.Information("Mock user registration: {Email}", request.Email);
        return true;
    }

    public async Task<bool> ConfirmRegistrationAsync(string email, string confirmationCode)
    {
        // Mock successful confirmation
        await Task.Delay(100);
        _logger.Information("Mock registration confirmation: {Email}", email);
        return true;
    }
}