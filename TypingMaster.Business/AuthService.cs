using Serilog;
using TypingMaster.Business.Contract;
using TypingMaster.Core.Models;
using TypingMaster.Core.Utility;

namespace TypingMaster.Business;

public class AuthService(
    HttpClient httpClient,
    IAccountService accountService,
    ILoginLogService loginLogService,
    IIdpService idpService,
    JwtTokenGenerator tokenGenerator,
    ILogger logger)
    : IAuthService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly JwtTokenGenerator _tokenGenerator = tokenGenerator;

    public ProcessResult ProcessResult { get; set; } = new(logger);

    public async Task<AuthResponse> Login(string email, string password)
    {
        try
        {
            // Retrieve account information based on email
            var account = await accountService.GetAccountByEmail(email);
            if (account == null)
            {
                await loginLogService.CreateLoginLog(0, null, null, false, "Account not found");
                return new AuthResponse
                {
                    Success = false,
                    Message = "Account not found"
                };
            }

            // Use mock IDP service for authentication
            var idpResponse = await idpService.Authenticate(account.AccountName, password);
            if (!idpResponse.Success)
            {
                await loginLogService.CreateLoginLog(account.Id, null, null, false, "IDP authentication failed");
                return new AuthResponse
                {
                    Success = false,
                    Message = "Authentication failed"
                };
            }

            // Log the successful login attempt
            await loginLogService.CreateLoginLog(account.Id, null, null, true);

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
            await loginLogService.CreateLoginLog(0, null, null, false, $"Error during login: {ex.Message}");
            return new AuthResponse
            {
                Success = false,
                Message = "An error occurred during login"
            };
        }
    }

    public Task<bool> Logout(int accountId, string refreshToken)
    {
        // Implement logout logic
        return Task.FromResult(true);
    }

    public async Task<AuthResponse> Register(RegisterRequest request)
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
            // Parallelize email check and IDP registration
            var emailCheckTask = accountService.GetAccountByEmail(request.Email);
            var idpRegistrationTask = idpService.RegisterUser(request);

            var existingAccount = await emailCheckTask;
            if (existingAccount != null)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Email is already registered"
                };
            }

            var idpRegistrationSuccess = await idpRegistrationTask;
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

    public async Task<AuthResponse> RefreshToken(string token, string refreshToken, string email)
    {
        try
        {
            // Use task to perform IDP refresh and account lookup in parallel
            var idpResponseTask = idpService.RefreshToken(refreshToken, email);

            // Only attempt account lookup if we have an email
            Task<Account?> accountTask = Task.FromResult<Account?>(null);
            if (!string.IsNullOrEmpty(email))
            {
                accountTask = accountService.GetAccountByEmail(email);
            }

            // Wait for both tasks to complete
            await Task.WhenAll(idpResponseTask, accountTask);

            var idpResponse = await idpResponseTask;
            if (!idpResponse.Success)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = idpResponse.Message ?? "Failed to refresh token"
                };
            }

            // Get the account result from the parallel task
            var account = await accountTask;

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

    public Task<bool> ChangePassword(int accountId, string currentPassword, string newPassword)
    {
        // Temporarily disabled password change functionality
        return Task.FromResult(true);
    }

    public Task<bool> RequestPasswordReset(string email)
    {
        // Temporarily disabled password reset functionality
        return Task.FromResult(true);
    }

    public Task<bool> ResetPassword(string token, string newPassword)
    {
        // Temporarily disabled password reset functionality
        return Task.FromResult(true);
    }

    public async Task<bool> ConfirmEmail(string token)
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
            var success = await idpService.ConfirmRegistration(email, confirmationCode);
            if (!success)
            {
                return false;
            }

            // Update the account status
            var account = await accountService.GetAccountByEmail(email);
            if (account != null)
            {
                await accountService.SetAccountStatus(account.Id, true);
            }

            return true;
        }
        catch (Exception ex)
        {
            ProcessResult.AddException(ex);
            return false;
        }
    }

    public Task<bool> ResendConfirmationEmail(string email)
    {
        // Temporarily disabled email confirmation
        return Task.FromResult(true);
    }

    public async Task<bool> ResendConfirmationCode(string userName)
    {
        try
        {
            var success = await idpService.ResendConfirmationCode(userName);
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

    public async Task<bool> ConfirmRegistration(string userName, string confirmationCode)
    {
        try
        {
            var success = await idpService.ConfirmRegistration(userName, confirmationCode);
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