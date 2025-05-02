using System.Net.Http.Json;
using TypingMaster.Core.Models;

namespace TypingMaster.Client.Services;

public class AuthWebService(HttpClient httpClient,
    IApiConfiguration apiConfig,
    IAccountWebService accountService,
    IPracticeLogWebService practiceLogService,
    ICourseWebService courseService,
    ApplicationContext appState,
    Serilog.ILogger logger) : IAuthWebService
{
    public async Task<AuthResponse> LoginAsync(string email, string password, bool rememberMe = false)
    {
        try
        {
            var loginRequest = new { Email = email, Password = password, RememberMe = rememberMe };
            var url = apiConfig.BuildApiUrl($"{apiConfig.ApiSettings.AuthService}/login");
            var response = await httpClient.PostAsJsonAsync(url, loginRequest);

            response.EnsureSuccessStatusCode();
            var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>()
                   ?? new AuthResponse { Success = false, Message = "Failed to deserialize response" };
            if (authResponse.Success)
            {
                var authResult = await AuthenticateUserAsync(authResponse, rememberMe);
                if (!authResult.Success)
                {
                    authResponse.Success = false;
                    authResponse.Message = authResult.Message;
                    authResult.StatusCode = authResult.StatusCode;
                }
            }

            return authResponse;
        }
        catch (Exception ex)
        {
            return new AuthResponse
            {
                Success = false,
                Message = $"Login failed: {ex.Message}"
            };
        }
    }

    public async Task<WebServiceResponse> LogoutAsync(int accountId, string refreshToken)
    {
        try
        {
            var logoutRequest = new { AccountId = accountId, RefreshToken = refreshToken };
            var url = apiConfig.BuildApiUrl($"{apiConfig.ApiSettings.AuthService}/logout");
            var response = await httpClient.PostAsJsonAsync(url, logoutRequest);

            return new WebServiceResponse
            {
                Success = response.IsSuccessStatusCode,
                StatusCode = response.StatusCode,
                Message = response.IsSuccessStatusCode ? "Logout successful" : $"Logout failed: {response.StatusCode}"
            };
        }
        catch (Exception ex)
        {
            return new WebServiceResponse
            {
                Success = false,
                StatusCode = System.Net.HttpStatusCode.InternalServerError,
                Message = $"Exception during logout: {ex.Message}"
            };
        }
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        try
        {
            var url = apiConfig.BuildApiUrl($"{apiConfig.ApiSettings.AuthService}/register");
            var response = await httpClient.PostAsJsonAsync(url, request);

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<AuthResponse>()
                   ?? new AuthResponse { Success = false, Message = "Failed to deserialize response" };
        }
        catch (Exception ex)
        {
            return new AuthResponse
            {
                Success = false,
                Message = $"Registration failed: {ex.Message}"
            };
        }
    }

    public async Task<AuthResponse> RefreshTokenAsync(string token, string refreshToken)
    {
        try
        {
            // Try to get the username from AppState if available
            string? accountEmail = appState.GetStoredAccountEmail();

            // If no email is available bad data found and user need login
            if (string.IsNullOrEmpty(accountEmail))
            {
                logger.Information("Email does not found, switch to login to fix the bad data");
                return new AuthResponse
                {
                    Success = false,
                    Message = "Invalid account data. Please log in again."
                };
            }

            var refreshRequest = new { Token = token, RefreshToken = refreshToken, Email = accountEmail };
            var url = apiConfig.BuildApiUrl($"{apiConfig.ApiSettings.AuthService}/refresh-token");

            // For token refresh, we need to set the Authorization header
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            logger.Information("Sending refresh token request to {Url}", url);
            var response = await httpClient.PostAsJsonAsync(url, refreshRequest);

            if (!response.IsSuccessStatusCode)
            {
                logger.Error($"Token refresh failed with status: {response.StatusCode}");

                // If we get a 401, try again without the Authorization header
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    logger.Information("Got 401, trying without Authorization header");
                    httpClient.DefaultRequestHeaders.Authorization = null;
                    response = await httpClient.PostAsJsonAsync(url, refreshRequest);

                    if (!response.IsSuccessStatusCode)
                    {
                        logger.Error($"Second token refresh attempt failed with status: {response.StatusCode}");
                        return new AuthResponse
                        {
                            Success = false,
                            Message = $"Token refresh failed with status: {response.StatusCode}"
                        };
                    }
                }
                else
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = $"Token refresh failed with status: {response.StatusCode}"
                    };
                }
            }

            var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>()
                    ?? new AuthResponse { Success = false, Message = "Failed to deserialize response" };

            // For auto-login purposes, if the refresh is successful but no account ID is provided,
            // we can extract the account ID from the existing token if available
            if (authResponse.Success && authResponse.AccountId == 0 && appState.CurrentAccount != null)
            {
                // Use the account ID from the current application state (if available)
                authResponse.AccountId = appState.CurrentAccount.Id;
            }

            return authResponse;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Token refresh failed: {Message}", ex.Message);
            return new AuthResponse
            {
                Success = false,
                Message = $"Token refresh failed: {ex.Message}"
            };
        }
    }

    public async Task<bool> ChangePasswordAsync(int accountId, string currentPassword, string newPassword)
    {
        try
        {
            var changePasswordRequest = new
            {
                AccountId = accountId,
                CurrentPassword = currentPassword,
                NewPassword = newPassword
            };

            var url = apiConfig.BuildApiUrl($"{apiConfig.ApiSettings.AuthService}/change-password");
            var response = await httpClient.PostAsJsonAsync(url, changePasswordRequest);

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<WebServiceResponse>();
            return result?.Success ?? false;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> RequestPasswordResetAsync(string email)
    {
        try
        {
            var resetRequest = new { Email = email };
            var url = apiConfig.BuildApiUrl($"{apiConfig.ApiSettings.AuthService}/forgot-password");
            var response = await httpClient.PostAsJsonAsync(url, resetRequest);

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<WebServiceResponse>();
            return result?.Success ?? false;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> ResetPasswordAsync(string token, string newPassword)
    {
        try
        {
            var resetPasswordRequest = new { Token = token, NewPassword = newPassword };
            var url = apiConfig.BuildApiUrl($"{apiConfig.ApiSettings.AuthService}/reset-password");
            var response = await httpClient.PostAsJsonAsync(url, resetPasswordRequest);

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<WebServiceResponse>();
            return result?.Success ?? false;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<WebServiceResponse> AuthenticateUserAsync(AuthResponse authResponse, bool rememberMe = false)
    {
        try
        {
            // Store the authentication token
            appState.Token = authResponse.Token;
            appState.RefreshToken = authResponse.RefreshToken;

            // Set the account in application state
            var account = await accountService.GetAccountAsync(authResponse.AccountId);
            if (account == null)
            {
                logger.Error("Cannot retrieve user information");
                var response = new WebServiceResponse
                {
                    Success = false,
                    Message = "Error retrieving user information"
                };
                return response;
            }
            appState.CurrentAccount = account;

            // Set persistence based on rememberMe flag
            appState.SetRememberMe(rememberMe);
            appState.SetAccountEmail(account.AccountEmail);


            // Load drill stats if available
            var drillStats = await practiceLogService.GetPaginatedDrillStatsAsync(account.History.Id);
            if (drillStats != null)
            {
                account.History.PracticeStats = drillStats.Items;
            }

            // Load course information if available
            if (appState.CurrentAccount.CourseId != Guid.Empty)
            {
                appState.CurrentCourse = await courseService.GetCourse(appState.CurrentAccount.CourseId);
            }

            appState.IsLoggedIn = true;
            return new WebServiceResponse { Success = true };
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error during authentication");
            return new WebServiceResponse { Success = false, Message = "An error occurred during login." };
        }
    }

    public async Task<bool> ResendConfirmationCodeAsync(string userName)
    {
        try
        {
            var request = new { UserName = userName };
            var url = apiConfig.BuildApiUrl($"{apiConfig.ApiSettings.AuthService}/resend-confirmation");
            var response = await httpClient.PostAsJsonAsync(url, request);

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<WebServiceResponse>();
            return result?.Success ?? false;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error resending confirmation code");
            return false;
        }
    }

    public async Task<bool> ConfirmRegistrationAsync(string userName, string confirmationCode)
    {
        try
        {
            var request = new { UserName = userName, ConfirmationCode = confirmationCode };
            var url = apiConfig.BuildApiUrl($"{apiConfig.ApiSettings.AuthService}/confirm-registration");
            var response = await httpClient.PostAsJsonAsync(url, request);

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<WebServiceResponse>();
            return result?.Success ?? false;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error confirming registration");
            return false;
        }
    }

    public async Task<bool> TryAutoLoginAsync(string token, string refreshToken)
    {
        try
        {
            logger.Information("Attempting auto-login with stored tokens");

            // Try to get the username from storage if not already available
            string? accountEmail = appState.GetStoredAccountEmail();

            // Check if email is missing, which indicates invalid account data
            if (string.IsNullOrEmpty(accountEmail))
            {
                logger.Warning("Auto-login failed: Email not found in account data");
                return false;
            }

            // First try to refresh the token
            var refreshResponse = await RefreshTokenAsync(token, refreshToken);
            if (!refreshResponse.Success)
            {
                logger.Error("Auto-login failed: Token refresh failed");
                return false;
            }

            // If we don't have an account ID in the response, auto-login can't continue
            if (refreshResponse.AccountId == 0)
            {
                // If the refresh was successful, but we don't have an account ID,
                // we might need to make a separate API call to retrieve user info
                logger.Warning("Auto-login: No account ID in refresh response, attempting to retrieve user info");

                // If we have a username from the refresh, we could try to get the account that way
                if (!string.IsNullOrEmpty(refreshResponse.AccountName))
                {
                    logger.Information("Using accountName from refresh response: {AccountName}", refreshResponse.AccountName);
                    // Save the username for future auto-login attempts
                    appState.SaveUserName(refreshResponse.AccountName);
                }
            }

            // Now authenticate the user with the refreshed token
            var authResult = await AuthenticateUserAsync(refreshResponse, true);
            if (authResult.Success)
            {
                logger.Information("Auto-login successful");
                return true;
            }

            logger.Error("Auto-login failed: Authentication step failed");
            return false;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error during auto-login");
            return false;
        }
    }
}