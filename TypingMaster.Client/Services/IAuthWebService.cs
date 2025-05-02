using TypingMaster.Core.Models;

namespace TypingMaster.Client.Services
{
    public interface IAuthWebService
    {
        Task<AuthResponse> LoginAsync(string email, string password, bool rememberMe = false);

        Task<WebServiceResponse> LogoutAsync(int accountId, string refreshToken);

        Task<AuthResponse> RegisterAsync(RegisterRequest request);

        Task<AuthResponse> RefreshTokenAsync(string token, string refreshToken);

        Task<bool> ChangePasswordAsync(int accountId, string currentPassword, string newPassword);

        Task<bool> RequestPasswordResetAsync(string email);

        Task<bool> ResetPasswordAsync(string token, string newPassword);

        Task<bool> ResendConfirmationCodeAsync(string userName);

        /// <summary>
        /// Authenticates a user and updates application state with user information
        /// </summary>
        /// <param name="authResponse">Response of authentication</param>
        /// <param name="rememberMe">if user information is saved to local</param>
        /// <returns>Tuple containing success status and error message if applicable</returns>
        Task<WebServiceResponse> AuthenticateUserAsync(AuthResponse authResponse, bool rememberMe = false);

        /// <summary>
        /// Confirms a user's registration using the confirmation code.
        /// </summary>
        /// <param name="userName">The username of the user to confirm.</param>
        /// <param name="confirmationCode">The confirmation code received via email.</param>
        /// <returns>A task representing the asynchronous operation, with a boolean indicating success or failure.</returns>
        Task<bool> ConfirmRegistrationAsync(string userName, string confirmationCode);

        /// <summary>
        /// Attempts to automatically log in the user using stored tokens
        /// </summary>
        /// <param name="token">The stored JWT token</param>
        /// <param name="refreshToken">The stored refresh token</param>
        /// <returns>True if auto-login was successful, false otherwise</returns>
        Task<bool> TryAutoLoginAsync(string token, string refreshToken);
    }
}