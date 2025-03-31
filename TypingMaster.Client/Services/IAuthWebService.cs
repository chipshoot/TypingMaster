using TypingMaster.Core.Models;

namespace TypingMaster.Client.Services
{
    public interface IAuthWebService
    {
        Task<AuthResponse> LoginAsync(string email, string password);

        Task<WebServiceResponse> LogoutAsync(int accountId, string refreshToken);

        Task<AuthResponse> RegisterAsync(RegisterRequest request);

        Task<AuthResponse> RefreshTokenAsync(string token, string refreshToken);

        Task<bool> ChangePasswordAsync(int accountId, string currentPassword, string newPassword);

        Task<bool> RequestPasswordResetAsync(string email);

        Task<bool> ResetPasswordAsync(string token, string newPassword);

        /// <summary>
        /// Authenticates a user and updates application state with user information
        /// </summary>
        /// <param name="authResponse">Response of authentication</param>
        /// <returns>Tuple containing success status and error message if applicable</returns>
        Task<WebServiceResponse> AuthenticateUserAsync(AuthResponse authResponse);
    }
}