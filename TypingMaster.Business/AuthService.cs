using Serilog;
using TypingMaster.Business.Contract;
using TypingMaster.Core.Models;

namespace TypingMaster.Business;

public class AuthService : ServiceBase, IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly IAccountService _accountService;
    private readonly ILoginLogService _loginLogService;
    private readonly IIdpService _idpService;
    private readonly JwtTokenGenerator _tokenGenerator;

    public AuthService(HttpClient httpClient, IAccountService accountService,
        ILoginLogService loginLogService, IIdpService idpService,
        JwtTokenGenerator tokenGenerator, ILogger logger)
        : base(logger)
    {
        _httpClient = httpClient;
        _accountService = accountService;
        _loginLogService = loginLogService;
        _idpService = idpService;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<AuthResponse> LoginAsync(string email, string password)
    {
        try
        {
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

            // Use mock IDP service for authentication
            var idpResponse = await _idpService.AuthenticateAsync(email, password);
            if (!idpResponse.Success)
            {
                await _loginLogService.CreateLoginLogAsync(account.Id, null, null, false, "IDP authentication failed");
                return new AuthResponse
                {
                    Success = false,
                    Message = "Authentication failed"
                };
            }

            // Log the successful login attempt
            await _loginLogService.CreateLoginLogAsync(account.Id, null, null, true);

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
            var existingAccount = await _accountService.GetAccountByEmail(request.Email);
            if (existingAccount != null)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Email is already registered"
                };
            }

            // Register with the identity provider
            var idpRegistrationSuccess = await _idpService.RegisterUserAsync(request);
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
            var createdAccount = await _accountService.CreateAccount(newAccount);

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

    public async Task<AuthResponse> RefreshTokenAsync(string token, string refreshToken)
    {
        try
        {
            var idpResponse = await _idpService.RefreshTokenAsync(refreshToken);
            return new AuthResponse
            {
                Success = true,
                Token = idpResponse.AccessToken,
                RefreshToken = idpResponse.RefreshToken
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
            var success = await _idpService.ConfirmRegistrationAsync(email, confirmationCode);
            if (!success)
            {
                return false;
            }

            // Update the account status
            var account = await _accountService.GetAccountByEmail(email);
            if (account != null)
            {
                await _accountService.SetAccountStatusAsync(account.Id, true);
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
}