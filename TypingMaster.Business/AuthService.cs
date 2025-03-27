using Serilog;
using TypingMaster.Business.Contract;
using TypingMaster.Core.Models;

namespace TypingMaster.Business;

public class AuthService : ServiceBase, IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly IAccountService _accountService;
    private readonly ILoginLogService _loginLogService;
    private readonly MockIdpService _idpService;
    private readonly JwtTokenGenerator _tokenGenerator;

    public AuthService(HttpClient httpClient, IAccountService accountService,
        ILoginLogService loginLogService, MockIdpService idpService,
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
            // Create a new account from the registration request
            var newAccount = new Account
            {
                AccountName = request.AccountName,
                AccountEmail = request.Email,
                User = new UserProfile
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                },
                GoalStats = new StatsBase { Wpm = 0, Accuracy = 0 }
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

            // Use mock IDP service for authentication
            var idpResponse = await _idpService.AuthenticateAsync(request.Email, request.Password);

            // Return success response with account info and token
            return new AuthResponse
            {
                Success = true,
                Message = "Registration successful",
                Token = idpResponse.AccessToken,
                RefreshToken = idpResponse.RefreshToken,
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

    public Task<bool> ConfirmEmailAsync(string token)
    {
        // Temporarily disabled email confirmation
        return Task.FromResult(true);
    }

    public Task<bool> ResendConfirmationEmailAsync(string email)
    {
        // Temporarily disabled email confirmation
        return Task.FromResult(true);
    }
}