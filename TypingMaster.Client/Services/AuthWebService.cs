using System.Net.Http.Json;
using TypingMaster.Client.Models;
using TypingMaster.Core.Models;
using AuthResponse = TypingMaster.Client.Models.AuthResponse;

namespace TypingMaster.Client.Services;

public class AuthWebService : IAuthWebService
{
    private readonly HttpClient _httpClient;
    private readonly IApiConfiguration _apiConfig;
    private const string BaseUrl = "api/auth";

    public AuthWebService(HttpClient httpClient, IApiConfiguration apiConfig)
    {
        _httpClient = httpClient;
        _apiConfig = apiConfig;
    }

    public async Task<AuthResponse> LoginAsync(string email, string password)
    {
        try
        {
            var loginRequest = new { Email = email, Password = password };
            var url = _apiConfig.BuildApiUrl($"{BaseUrl}/login");
            var response = await _httpClient.PostAsJsonAsync(url, loginRequest);

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<AuthResponse>()
                   ?? new AuthResponse { Success = false, Message = "Failed to deserialize response" };
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
            var logoutRequest = new { AccountId = accountId, RefreshToken = refreshToken ?? string.Empty };
            var url = _apiConfig.BuildApiUrl($"{BaseUrl}/logout");
            var response = await _httpClient.PostAsJsonAsync(url, logoutRequest);

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
            var url = _apiConfig.BuildApiUrl($"{BaseUrl}/register");
            var response = await _httpClient.PostAsJsonAsync(url, request);

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
            var url = _apiConfig.BuildApiUrl($"{BaseUrl}/refresh-token");
            var response = await _httpClient.PostAsJsonAsync(url, refreshRequest);

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

            var url = _apiConfig.BuildApiUrl($"{BaseUrl}/change-password");
            var response = await _httpClient.PostAsJsonAsync(url, changePasswordRequest);

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<SuccessResponse>();
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
            var url = _apiConfig.BuildApiUrl($"{BaseUrl}/forgot-password");
            var response = await _httpClient.PostAsJsonAsync(url, resetRequest);

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<SuccessResponse>();
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
            var url = _apiConfig.BuildApiUrl($"{BaseUrl}/reset-password");
            var response = await _httpClient.PostAsJsonAsync(url, resetPasswordRequest);

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<SuccessResponse>();
            return result?.Success ?? false;
        }
        catch (Exception)
        {
            return false;
        }
    }

    // Helper class for simple success responses
    private class SuccessResponse
    {
        public bool Success { get; set; }
    }
}