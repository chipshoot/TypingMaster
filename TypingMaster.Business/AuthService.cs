using Serilog;
using System.Text;
using System.Text.Json;
using TypingMaster.Business.Contract;
using TypingMaster.Business.Models;

namespace TypingMaster.Business;

public class AuthService : ServiceBase, IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly IAccountService _accountService;
    private readonly ILoginLogService _loginLogService;
    private readonly ILoginCredentialService _credentialService;
    private readonly JwtTokenGenerator _tokenGenerator;

    public AuthService(HttpClient httpClient, IAccountService accountService,
        ILoginLogService loginLogService, ILoginCredentialService credentialService,
        JwtTokenGenerator tokenGenerator, ILogger logger)
        : base(logger)
    {
        _httpClient = httpClient;
        _accountService = accountService;
        _loginLogService = loginLogService;
        _credentialService = credentialService;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<AuthResponse> LoginAsync(string email, string password)
    {
        try
        {
            // First, check if account is locked
            bool isLocked = await _credentialService.IsAccountLockedAsync(email);
            if (isLocked)
            {
                await _loginLogService.CreateLoginLogAsync(0, null, null, false, "Account is locked");
                return new AuthResponse
                {
                    Success = false,
                    Message = "This account is temporarily locked due to too many failed login attempts. Please try again later."
                };
            }

            // Validate credentials
            var isValidCredentials = await _credentialService.ValidateCredentialsAsync(email, password);
            if (!isValidCredentials)
            {
                await _loginLogService.CreateLoginLogAsync(0, null, null, false, "Invalid email or password");
                return new AuthResponse
                {
                    Success = false,
                    Message = "Invalid email or password"
                };
            }

            // Retrieve account information based on email
            var account = await _accountService.GetAccountByEmail(email);
            if (account == null)
            {
                await _loginLogService.CreateLoginLogAsync(0, null, null, false, "Account not found");
                return new AuthResponse
                {
                    Success = false,
                    Message = "Account not found"
                };
            }

            // Generate JWT token
            string token;
            // For local development/testing, we can use a fake token
            var useExternalIdp = false;  // This would be a configuration setting

            if (useExternalIdp)
            {
                // Get token from external IdP
                token = await AuthenticateWithIdpAsync(email, password);
                if (string.IsNullOrEmpty(token))
                {
                    await _loginLogService.CreateLoginLogAsync(account.Id, null, null, false, "Failed to obtain token");
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Authentication failed"
                    };
                }
            }
            else
            {
                // Generate JWT token
                token = _tokenGenerator.GenerateJwtToken(account);
            }

            // Log the successful login attempt
            await _loginLogService.CreateLoginLogAsync(account.Id, null, null, true);

            return new AuthResponse
            {
                Success = true,
                Token = token,
                AccountId = account.Id,
                AccountName = account.AccountName
            };
        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);

            // Log the error during login
            await _loginLogService.CreateLoginLogAsync(0, null, null, false, $"Error during login: {ex.Message}");

            return new AuthResponse
            {
                Success = false,
                Message = "An error occurred during login"
            };
        }
    }

    public Task<bool> LogoutAsync(int accountId, string refreshToken)
    {
        // Implement logout logic
        // Clear refresh token in the database if using refresh tokens
        return Task.FromResult(true);
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        if (request == null)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Registration request cannot be null"
            };
        }

        try
        {
            // Create a new account from the registration request
            var newAccount = new Account
            {
                AccountName = request.AccountName,
                AccountEmail = request.Email,
                User = new UserProfile
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                }
            };

            // Check if email is already registered
            var existingAccount = await _accountService.GetAccountByEmail(request.Email);
            if (existingAccount != null)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Email is already registered"
                };
            }

            // Save the account using the account service
            var createdAccount = await _accountService.CreateAccount(newAccount);

            if (createdAccount == null)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Failed to create account"
                };
            }

            // Create login credentials
            await _credentialService.CreateAsync(createdAccount.Id, request.Email, request.Password);

            // Generate email confirmation token
            var confirmationToken = await _credentialService.GenerateEmailConfirmationTokenAsync(createdAccount.Id);

            // In a real application, send an email with the confirmation link
            // emailService.SendConfirmationEmail(request.Email, confirmationToken);

            // Generate authentication token
            string token;
            var useExternalIdp = false;  // This would be a configuration setting

            if (useExternalIdp)
            {
                // Get token from external IdP
                token = await AuthenticateWithIdpAsync(request.Email, request.Password);
            }
            else
            {
                // Generate JWT token
                token = _tokenGenerator.GenerateJwtToken(createdAccount);
            }

            // Return success response with account info and token
            return new AuthResponse
            {
                Success = true,
                Message = "Registration successful",
                Token = token,
                AccountId = createdAccount.Id,
                AccountName = createdAccount.AccountName
            };
        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);
            return new AuthResponse
            {
                Success = false,
                Message = "An error occurred during registration"
            };
        }
    }

    public async Task<AuthResponse> RefreshTokenAsync(string token, string refreshToken)
    {
        // Implementation for refreshing token
        // In a real-world scenario, this would validate the refresh token and issue a new access token

        return new AuthResponse { Success = false, Message = "Not fully implemented" };
    }

    public async Task<bool> ChangePasswordAsync(int accountId, string currentPassword, string newPassword)
    {
        // Use the credential service to change the password
        return await _credentialService.ChangePasswordAsync(accountId, currentPassword, newPassword);
    }

    public async Task<bool> RequestPasswordResetAsync(string email)
    {
        try
        {
            // Generate a password reset token
            var token = await _credentialService.GeneratePasswordResetTokenAsync(email);

            if (string.IsNullOrEmpty(token))
            {
                return false;
            }

            // In a real application, send an email with the reset link
            // emailService.SendPasswordResetEmail(email, token);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> ResetPasswordAsync(string token, string newPassword)
    {
        return await _credentialService.ResetPasswordAsync(token, newPassword);
    }

    public async Task<bool> ConfirmEmailAsync(string token)
    {
        return await _credentialService.ConfirmEmailAsync(token);
    }

    public async Task<bool> ResendConfirmationEmailAsync(string email)
    {
        try
        {
            var account = await _accountService.GetAccountByEmail(email);
            if (account == null)
            {
                return false;
            }

            var token = await _credentialService.GenerateEmailConfirmationTokenAsync(account.Id);

            if (string.IsNullOrEmpty(token))
            {
                return false;
            }

            // In a real application, send an email with the confirmation link
            // emailService.SendConfirmationEmail(email, token);

            return true;
        }
        catch
        {
            return false;
        }
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