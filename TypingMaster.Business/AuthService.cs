using Serilog;
using TypingMaster.Business.Contract;
using TypingMaster.Core.Models;

namespace TypingMaster.Business;

public class AuthService(
    HttpClient httpClient,
    IAccountService accountService,
    ILoginLogService loginLogService,
    IIdpService idpService,
    JwtTokenGenerator tokenGenerator,
    ILogger logger)
    : ServiceBase(logger), IAuthService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly JwtTokenGenerator _tokenGenerator = tokenGenerator;

    public async Task<AuthResponse> LoginAsync(string email, string password)
    {
        try
        {
            // Retrieve account information based on email
            var account = await accountService.GetAccountByEmail(email);
            if (account == null)
            {
                await loginLogService.CreateLoginLogAsync(0, null, null, false, "Account not found");
                return new AuthResponse
                {
                    Success = false,
                    Message = "Account not found"
                };
            }

            // Use mock IDP service for authentication
            var idpResponse = await idpService.AuthenticateAsync(account.AccountName, password);
            if (!idpResponse.Success)
            {
                await loginLogService.CreateLoginLogAsync(account.Id, null, null, false, "IDP authentication failed");
                return new AuthResponse
                {
                    Success = false,
                    Message = "Authentication failed"
                };
            }

            // Log the successful login attempt
            await loginLogService.CreateLoginLogAsync(account.Id, null, null, true);

            return new AuthResponse
            {
                Success = true,
                Token = idpResponse.AccessToken,
                RefreshToken = idpResponse.RefreshToken,
                AccountId = account.Id,
                AccountName = account.AccountName
            };
        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);
            await loginLogService.CreateLoginLogAsync(0, null, null, false, $"Error during login: {ex.Message}");
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
            // Check if email is already registered
            var existingAccount = await accountService.GetAccountByEmail(request.Email);
            if (existingAccount != null)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Email is already registered"
                };
            }

            // Register with the identity provider
            var idpRegistrationSuccess = await idpService.RegisterUserAsync(request);
            if (!idpRegistrationSuccess)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Failed to register with identity provider"
                };
            }

            // Create a new account from the registration request
            var newAccount = new Account
            {
                AccountName = request.AccountName,
                AccountEmail = request.Email,
                User = new UserProfile
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName ?? string.Empty,
                },
                GoalStats = new StatsBase { Wpm = 0, Accuracy = 0 }
            };

            // Save the account using the account service
            var createdAccount = await accountService.CreateAccount(newAccount);

            if (createdAccount == null)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Failed to create account"
                };
            }

            // Return success response with account info
            return new AuthResponse
            {
                Success = true,
                Message = "Registration successful. Please check your email for confirmation code.",
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

    public async Task<AuthResponse> RefreshTokenAsync(string token, string refreshToken, string email)
    {
        try
        {
            var idpResponse = await idpService.RefreshTokenAsync(refreshToken, email);
            if (!idpResponse.Success)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = idpResponse.Message ?? "Failed to refresh token"
                };
            }

            // Try to get account information if we have a username
            Account? account = null;
            if (!string.IsNullOrEmpty(email))
            {
                account = await accountService.GetAccountByEmail(email);
            }

            return new AuthResponse
            {
                Success = true,
                Token = idpResponse.AccessToken,
                RefreshToken = idpResponse.RefreshToken,
                AccountId = account?.Id ?? 0,
                AccountName = account?.AccountName ?? string.Empty
            };
        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);
            return new AuthResponse
            {
                Success = false,
                Message = "Failed to refresh token"
            };
        }
    }

    public Task<bool> ChangePasswordAsync(int accountId, string currentPassword, string newPassword)
    {
        // Temporarily disabled password change functionality
        return Task.FromResult(true);
    }

    public Task<bool> RequestPasswordResetAsync(string email)
    {
        // Temporarily disabled password reset functionality
        return Task.FromResult(true);
    }

    public Task<bool> ResetPasswordAsync(string token, string newPassword)
    {
        // Temporarily disabled password reset functionality
        return Task.FromResult(true);
    }

    public async Task<bool> ConfirmEmailAsync(string token)
    {
        try
        {
            // Extract email and confirmation code from the token
            // This assumes the token is in the format "email:confirmationCode"
            var parts = token.Split(':');
            if (parts.Length != 2)
            {
                return false;
            }

            var email = parts[0];
            var confirmationCode = parts[1];

            // Confirm the registration with the identity provider
            var success = await idpService.ConfirmRegistrationAsync(email, confirmationCode);
            if (!success)
            {
                return false;
            }

            // Update the account status
            var account = await accountService.GetAccountByEmail(email);
            if (account != null)
            {
                await accountService.SetAccountStatusAsync(account.Id, true);
            }

            return true;
        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);
            return false;
        }
    }

    public Task<bool> ResendConfirmationEmailAsync(string email)
    {
        // Temporarily disabled email confirmation
        return Task.FromResult(true);
    }

    public async Task<bool> ResendConfirmationCodeAsync(string userName)
    {
        try
        {
            var success = await idpService.ResendConfirmationCodeAsync(userName);
            if (!success)
            {
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);
            return false;
        }
    }

    public async Task<bool> ConfirmRegistrationAsync(string userName, string confirmationCode)
    {
        try
        {
            var success = await idpService.ConfirmRegistrationAsync(userName, confirmationCode);
            if (!success)
            {
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);
            return false;
        }
    }
}