using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Microsoft.Extensions.Options;
using Serilog;
using TypingMaster.Business.Contract;
using TypingMaster.Business.Config;
using TypingMaster.Core.Models;
using System.Security.Cryptography;
using System.Text;

namespace TypingMaster.Business;

public class AwsCognitoService(
    IAmazonCognitoIdentityProvider cognitoClient,
    IOptions<CognitoSettings> cognitoSettings,
    ILogger logger)
    : IIdpService
{
    private readonly CognitoSettings _cognitoSettings = cognitoSettings.Value;

    private string CalculateSecretHash(string username)
    {
        var message = username + _cognitoSettings.ClientId;
        var keyBytes = Encoding.UTF8.GetBytes(_cognitoSettings.ClientSecret);
        var messageBytes = Encoding.UTF8.GetBytes(message);

        using var hmac = new HMACSHA256(keyBytes);
        var hash = hmac.ComputeHash(messageBytes);
        return Convert.ToBase64String(hash);
    }

    public async Task<IdpAuthResponse> AuthenticateAsync(string userName, string password)
    {
        try
        {
            var authRequest = new InitiateAuthRequest
            {
                ClientId = _cognitoSettings.ClientId,
                AuthFlow = AuthFlowType.USER_PASSWORD_AUTH,
                AuthParameters = new Dictionary<string, string>
                {
                    { "USERNAME", userName },
                    { "PASSWORD", password },
                    { "SECRET_HASH", CalculateSecretHash(userName) }
                }
            };

            logger.Information("Initiating authentication for user: {UserName}", userName);
            var response = await cognitoClient.InitiateAuthAsync(authRequest);

            if (response.ChallengeName == "NEW_PASSWORD_REQUIRED")
            {
                // Prompt the user for a new password
                var newPassword = "Password2@"; // Get this from your UI
                var challengeResponse = await RespondToNewPasswordChallengeAsync(
                    userName,
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
                logger.Warning("Authentication result is null. Challenge: {Challenge}, Session: {Session}",
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
            logger.Error(ex, "Error authenticating with Cognito for user: {Email}", userName);
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

            logger.Information("Responding to new password challenge for user: {Email}", email);
            var response = await cognitoClient.RespondToAuthChallengeAsync(challengeRequest);

            if (response.AuthenticationResult == null)
            {
                logger.Error("Failed to set new password. Challenge response: {Challenge}", response.ChallengeName);
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
            logger.Error(ex, "Error responding to new password challenge for user: {Email}", email);
            return new IdpAuthResponse
            {
                Success = false,
                Message = ex.Message
            };
        }
    }

    public async Task<IdpAuthResponse> RefreshTokenAsync(string refreshToken, string userName)
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
                    { "SECRET_HASH", CalculateSecretHash(userName) }
                }
            };

            logger.Information("Initiating token refresh");
            var response = await cognitoClient.InitiateAuthAsync(authRequest);

            logger.Information("Refresh response: Challenge={Challenge}, Session={Session}, AuthenticationResult={AuthResult}",
                response.ChallengeName,
                response.Session,
                response.AuthenticationResult != null ? "Present" : "Null");

            if (response.AuthenticationResult == null)
            {
                logger.Error("Authentication result is null during token refresh. Challenge: {Challenge}", response.ChallengeName);
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
            logger.Error(ex, "Error refreshing token with Cognito");
            return new IdpAuthResponse
            {
                Success = false,
                Message = ex.Message
            };
        }
    }
    public async Task<bool> ResendConfirmationCodeAsync(string userName)
    {
        try
        {
            var request = new ResendConfirmationCodeRequest
            {
                ClientId = _cognitoSettings.ClientId,
                Username = userName,
                SecretHash = CalculateSecretHash(userName)
            };

            await cognitoClient.ResendConfirmationCodeAsync(request);
            logger.Information("Verification code resent for user: {Username}", userName);
            return true;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error resending verification code: {Error}", ex.Message);
            return false;
        }
    }

    public async Task<bool> RegisterUserAsync(RegisterRequest request)
    {
        try
        {
            var signUpRequest = new SignUpRequest
            {
                ClientId = _cognitoSettings.ClientId,
                Username = request.AccountName,
                Password = request.Password,
                SecretHash = CalculateSecretHash(request.AccountName),
                UserAttributes =
                [
                    new AttributeType { Name = "email", Value = request.Email },
                    new AttributeType { Name = "given_name", Value = request.FirstName },
                    new AttributeType { Name = "name", Value = request.LastName }
                ]
            };

            var response = await cognitoClient.SignUpAsync(signUpRequest);
            logger.Information("User registration initiated for {Email}", request.Email);
            return true;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error registering user with Cognito");
            return false;
        }
    }

    public async Task<bool> ConfirmRegistrationAsync(string userName, string confirmationCode)
    {
        try
        {
            var confirmSignUpRequest = new ConfirmSignUpRequest
            {
                ClientId = _cognitoSettings.ClientId,
                Username = userName,
                ConfirmationCode = confirmationCode,
                SecretHash = CalculateSecretHash(userName)
            };

            await cognitoClient.ConfirmSignUpAsync(confirmSignUpRequest);
            logger.Information("User registration confirmed for {Email}", userName);
            return true;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error confirming user registration with Cognito");
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

            await cognitoClient.AdminSetUserPasswordAsync(request);
            logger.Information("Set permanent password for user: {Email}", email);
            return true;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error setting permanent password for user: {Email}", email);
            return false;
        }
    }
}