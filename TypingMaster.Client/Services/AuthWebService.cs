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
    public async Task<AuthResponse> LoginAsync(string email, string password)
    {
        try
        {
            var loginRequest = new { Email = email, Password = password };
            var url = apiConfig.BuildApiUrl($"{apiConfig.ApiSettings.AuthService}/login");
            var response = await httpClient.PostAsJsonAsync(url, loginRequest);

            response.EnsureSuccessStatusCode();
            var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>()
                   ?? new AuthResponse { Success = false, Message = "Failed to deserialize response" };
            if (authResponse.Success)
            {
                var authResult = await AuthenticateUserAsync(authResponse);
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
            var refreshRequest = new { Token = token, RefreshToken = refreshToken };
            var url = apiConfig.BuildApiUrl($"{apiConfig.ApiSettings.AuthService}/refresh-token");
            var response = await httpClient.PostAsJsonAsync(url, refreshRequest);

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<AuthResponse>()
                   ?? new AuthResponse { Success = false, Message = "Failed to deserialize response" };
        }
        catch (Exception ex)
        {
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

    public async Task<WebServiceResponse> AuthenticateUserAsync(AuthResponse authResponse)
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
}