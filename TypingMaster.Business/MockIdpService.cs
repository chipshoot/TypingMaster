using Serilog;
using TypingMaster.Business.Contract;
using TypingMaster.Core.Models;
using TypingMaster.Core.Utility;

namespace TypingMaster.Business;

public class MockIdpService(ILogger logger) : IIdpService
{
    public ProcessResult ProcessResult { get; set; } = new(logger);

    public async Task<IdpAuthResponse> Authenticate(string email, string password)
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

    public async Task<IdpAuthResponse> RespondToNewPasswordChallenge(string email, string newPassword, string session)
    {
        // Mock successful password change
        await Task.Delay(100);
        logger.Information("Mock new password challenge response for user: {Email}", email);

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

    public async Task<bool> SetPermanentPassword(string email, string password)
    {
        // Mock successful password setting
        await Task.Delay(100);
        logger.Information("Mock setting permanent password for user: {Email}", email);
        return true;
    }

    public async Task<IdpAuthResponse> RefreshToken(string refreshToken, string userName)
    {
        // Mock token refresh - removing delay for performance
        // Reduced delay from 100ms to 10ms to improve performance
        await Task.Delay(10);

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

    public async Task<bool> RegisterUser(RegisterRequest request)
    {
        // Mock successful registration
        await Task.Delay(10);
        logger.Information("Mock user registration: {Email}", request.Email);
        return true;
    }

    public async Task<bool> ConfirmRegistration(string email, string confirmationCode)
    {
        // Mock successful confirmation
        await Task.Delay(100);
        logger.Information("Mock registration confirmation: {Email}", email);
        return true;
    }

    public async Task<bool> ResendConfirmationCode(string userName)
    {
        // Mock successful code resend
        await Task.Delay(100);
        logger.Information("Mock resending confirmation code for user: {UserName}", userName);
        return true;
    }
}