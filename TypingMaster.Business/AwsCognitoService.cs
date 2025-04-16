using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Serilog;
using TypingMaster.Business.Contract;
using TypingMaster.Core.Models;

namespace TypingMaster.Business;

public class AwsCognitoService : IIdpService
{
    private readonly IAmazonCognitoIdentityProvider _cognitoClient;
    private readonly string _userPoolId;
    private readonly string _clientId;
    private readonly ILogger _logger;

    public AwsCognitoService(
        IAmazonCognitoIdentityProvider cognitoClient,
        string userPoolId,
        string clientId,
        ILogger logger)
    {
        _cognitoClient = cognitoClient;
        _userPoolId = userPoolId;
        _clientId = clientId;
        _logger = logger;
    }

    public async Task<IdpAuthResponse> AuthenticateAsync(string email, string password)
    {
        try
        {
            var authRequest = new InitiateAuthRequest
            {
                AuthFlow = AuthFlowType.USER_PASSWORD_AUTH,
                ClientId = _clientId,
                AuthParameters = new Dictionary<string, string>
                {
                    { "USERNAME", email },
                    { "PASSWORD", password }
                }
            };

            var response = await _cognitoClient.InitiateAuthAsync(authRequest);

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
            _logger.Error(ex, "Error authenticating with Cognito");
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
                ClientId = _clientId,
                AuthParameters = new Dictionary<string, string>
                {
                    { "REFRESH_TOKEN", refreshToken }
                }
            };

            var response = await _cognitoClient.InitiateAuthAsync(authRequest);

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
                ClientId = _clientId,
                Username = request.Email,
                Password = request.Password,
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
                ClientId = _clientId,
                Username = email,
                ConfirmationCode = confirmationCode
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
}