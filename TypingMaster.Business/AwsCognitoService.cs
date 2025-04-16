using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Microsoft.Extensions.Options;
using Serilog;
using TypingMaster.Business.Contract;
using TypingMaster.Business.Config;
using TypingMaster.Core.Models;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace TypingMaster.Business;

public class AwsCognitoService : IIdpService
{
    private readonly IAmazonCognitoIdentityProvider _cognitoClient;
    private readonly CognitoSettings _cognitoSettings;
    private readonly ILogger _logger;

    public AwsCognitoService(
        IAmazonCognitoIdentityProvider cognitoClient,
        IOptions<CognitoSettings> cognitoSettings,
        ILogger logger)
    {
        _cognitoClient = cognitoClient;
        _cognitoSettings = cognitoSettings.Value;
        _logger = logger;
    }

    private string CalculateSecretHash(string username)
    {
        var message = username + _cognitoSettings.ClientId;
        var keyBytes = Encoding.UTF8.GetBytes(_cognitoSettings.ClientSecret);
        var messageBytes = Encoding.UTF8.GetBytes(message);

        using var hmac = new HMACSHA256(keyBytes);
        var hash = hmac.ComputeHash(messageBytes);
        return Convert.ToBase64String(hash);
    }

    public async Task<IdpAuthResponse> AuthenticateAsync(string email, string password)
    {
        try
        {
            var authRequest = new InitiateAuthRequest
            {
                ClientId = _cognitoSettings.ClientId,
                AuthFlow = AuthFlowType.USER_PASSWORD_AUTH,
                AuthParameters = new Dictionary<string, string>
                {
                    { "USERNAME", email },
                    { "PASSWORD", password },
                    { "SECRET_HASH", CalculateSecretHash(email) }
                }
            };

            _logger.Information("Initiating authentication for user: {Email}", email);
            var response = await _cognitoClient.InitiateAuthAsync(authRequest);

            if (response.ChallengeName == "NEW_PASSWORD_REQUIRED")
            {
                // Prompt the user for a new password
                var newPassword = "Password2@"; // Get this from your UI
                var challengeResponse = await RespondToNewPasswordChallengeAsync(
                    email,
                    newPassword,
                    response.Session
                );

                if (challengeResponse.Success)
                {
                    return new IdpAuthResponse
                    {
                        Success = true,
                        AccessToken = challengeResponse.AccessToken,
                        TokenType = "Bearer",
                        ExpiresIn = challengeResponse.ExpiresIn,
                        RefreshToken = challengeResponse.RefreshToken,
                        Scope = "openid profile email"
                    };
                }
            }

            if (response.AuthenticationResult == null)
            {
                _logger.Warning("Authentication result is null. Challenge: {Challenge}, Session: {Session}",
                    response.ChallengeName,
                    response.Session);

                return new IdpAuthResponse
                {
                    Success = false,
                    Message = $"Authentication challenge: {response.ChallengeName}",
                    ChallengeName = response.ChallengeName?.ToString(),
                    Session = response.Session
                };
            }

            return new IdpAuthResponse
            {
                Success = true,
                AccessToken = response.AuthenticationResult.AccessToken,
                TokenType = "Bearer",
                ExpiresIn = response.AuthenticationResult.ExpiresIn,
                RefreshToken = response.AuthenticationResult.RefreshToken,
                Scope = "openid profile email"
            };
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error authenticating with Cognito for user: {Email}", email);
            return new IdpAuthResponse
            {
                Success = false,
                Message = ex.Message
            };
        }
    }

    public async Task<IdpAuthResponse> RespondToNewPasswordChallengeAsync(string email, string newPassword, string session)
    {
        try
        {
            var challengeRequest = new RespondToAuthChallengeRequest
            {
                ClientId = _cognitoSettings.ClientId,
                ChallengeName = ChallengeNameType.NEW_PASSWORD_REQUIRED,
                Session = session,
                ChallengeResponses = new Dictionary<string, string>
                {
                    { "USERNAME", email },
                    { "NEW_PASSWORD", newPassword },
                    { "SECRET_HASH", CalculateSecretHash(email) },
                    { "userAttributes.name", "fchy" },
                    { "userAttributes.given_name", "chaoyang" },
                }
            };

            _logger.Information("Responding to new password challenge for user: {Email}", email);
            var response = await _cognitoClient.RespondToAuthChallengeAsync(challengeRequest);

            if (response.AuthenticationResult == null)
            {
                _logger.Error("Failed to set new password. Challenge response: {Challenge}", response.ChallengeName);
                return new IdpAuthResponse
                {
                    Success = false,
                    Message = "Failed to set new password",
                    ChallengeName = response.ChallengeName?.ToString(),
                    Session = response.Session
                };
            }

            return new IdpAuthResponse
            {
                Success = true,
                AccessToken = response.AuthenticationResult.AccessToken,
                TokenType = "Bearer",
                ExpiresIn = response.AuthenticationResult.ExpiresIn,
                RefreshToken = response.AuthenticationResult.RefreshToken,
                Scope = "openid profile email"
            };
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error responding to new password challenge for user: {Email}", email);
            return new IdpAuthResponse
            {
                Success = false,
                Message = ex.Message
            };
        }
    }

    public async Task<IdpAuthResponse> RefreshTokenAsync(string refreshToken)
    {
        try
        {
            var authRequest = new InitiateAuthRequest
            {
                AuthFlow = AuthFlowType.REFRESH_TOKEN_AUTH,
                ClientId = _cognitoSettings.ClientId,
                AuthParameters = new Dictionary<string, string>
                {
                    { "REFRESH_TOKEN", refreshToken },
                    { "SECRET_HASH", CalculateSecretHash(refreshToken) }
                }
            };

            _logger.Information("Initiating token refresh");
            var response = await _cognitoClient.InitiateAuthAsync(authRequest);

            _logger.Information("Refresh response: Challenge={Challenge}, Session={Session}, AuthenticationResult={AuthResult}",
                response.ChallengeName,
                response.Session,
                response.AuthenticationResult != null ? "Present" : "Null");

            if (response.AuthenticationResult == null)
            {
                _logger.Error("Authentication result is null during token refresh. Challenge: {Challenge}", response.ChallengeName);
                return new IdpAuthResponse
                {
                    Success = false,
                    Message = "Failed to refresh token",
                    ChallengeName = response.ChallengeName?.ToString(),
                    Session = response.Session
                };
            }

            return new IdpAuthResponse
            {
                Success = true,
                AccessToken = response.AuthenticationResult.AccessToken,
                TokenType = "Bearer",
                ExpiresIn = response.AuthenticationResult.ExpiresIn,
                RefreshToken = refreshToken, // Keep the same refresh token
                Scope = "openid profile email"
            };
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error refreshing token with Cognito");
            return new IdpAuthResponse
            {
                Success = false,
                Message = ex.Message
            };
        }
    }

    public async Task<bool> RegisterUserAsync(RegisterRequest request)
    {
        try
        {
            var signUpRequest = new SignUpRequest
            {
                ClientId = _cognitoSettings.ClientId,
                Username = request.Email,
                Password = request.Password,
                SecretHash = CalculateSecretHash(request.Email),
                UserAttributes = new List<AttributeType>
                {
                    new AttributeType { Name = "email", Value = request.Email },
                    new AttributeType { Name = "given_name", Value = request.FirstName },
                    new AttributeType { Name = "family_name", Value = request.LastName ?? string.Empty }
                }
            };

            var response = await _cognitoClient.SignUpAsync(signUpRequest);
            _logger.Information("User registration initiated for {Email}", request.Email);
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error registering user with Cognito");
            return false;
        }
    }

    public async Task<bool> ConfirmRegistrationAsync(string email, string confirmationCode)
    {
        try
        {
            var confirmSignUpRequest = new ConfirmSignUpRequest
            {
                ClientId = _cognitoSettings.ClientId,
                Username = email,
                ConfirmationCode = confirmationCode,
                SecretHash = CalculateSecretHash(email)
            };

            await _cognitoClient.ConfirmSignUpAsync(confirmSignUpRequest);
            _logger.Information("User registration confirmed for {Email}", email);
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error confirming user registration with Cognito");
            return false;
        }
    }

    public async Task<bool> SetPermanentPasswordAsync(string email, string password)
    {
        try
        {
            var request = new AdminSetUserPasswordRequest
            {
                UserPoolId = _cognitoSettings.UserPoolId,
                Username = email,
                Password = password,
                Permanent = true
            };

            await _cognitoClient.AdminSetUserPasswordAsync(request);
            _logger.Information("Set permanent password for user: {Email}", email);
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error setting permanent password for user: {Email}", email);
            return false;
        }
    }
}