using TypingMaster.Core.Models;

namespace TypingMaster.Business.Contract;

/// <summary>
/// Interface to handle user authentication and authorization.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Logs in a user with the provided username and password.
    /// </summary>
    /// <param name="email">The username of the user.</param>
    /// <param name="password">The password of the user.</param>
    /// <returns>A task representing the asynchronous operation, with a <see cref="AuthResponse"/> result indicating success or failure.</returns>
    Task<AuthResponse> LoginAsync(string email, string password);

    /// <summary>
    /// Logs out the currently logged-in user.
    /// </summary>
    /// <param name="accountId">The ID of the account to log out.</param>
    /// <param name="refreshToken">The refresh token to invalidate.</param>
    /// <returns>A task representing the asynchronous operation, with a boolean indicating success or failure.</returns>
    Task<bool> LogoutAsync(int accountId, string refreshToken);

    /// <summary>
    /// Registers a new user with the provided registration request.
    /// </summary>
    /// <param name="request">The registration request containing user information.</param>
    /// <returns>A task representing the asynchronous operation, with a <see cref="AuthResponse"/> result indicating success or failure.</returns>
    Task<AuthResponse> RegisterAsync(RegisterRequest request);

    /// <summary>
    /// Refreshes an expired access token using a valid refresh token.
    /// </summary>
    /// <param name="token">The expired access token.</param>
    /// <param name="refreshToken">The refresh token to use for generating a new access token.</param>
    /// <returns>A task representing the asynchronous operation, with a <see cref="AuthResponse"/> containing the new tokens if successful.</returns>
    Task<AuthResponse> RefreshTokenAsync(string token, string refreshToken);

    /// <summary>
    /// Changes the password for an authenticated user.
    /// </summary>
    /// <param name="accountId">The ID of the account whose password is being changed.</param>
    /// <param name="currentPassword">The current password for verification.</param>
    /// <param name="newPassword">The new password to set.</param>
    /// <returns>A task representing the asynchronous operation, with a boolean indicating success or failure.</returns>
    Task<bool> ChangePasswordAsync(int accountId, string currentPassword, string newPassword);

    /// <summary>
    /// Initiates the password reset process by sending a reset link to the user's email.
    /// </summary>
    /// <param name="email">The email address of the account requesting password reset.</param>
    /// <returns>A task representing the asynchronous operation, with a boolean indicating success or failure.</returns>
    Task<bool> RequestPasswordResetAsync(string email);

    /// <summary>
    /// Completes the password reset process using a valid reset token.
    /// </summary>
    /// <param name="token">The password reset token received via email.</param>
    /// <param name="newPassword">The new password to set.</param>
    /// <returns>A task representing the asynchronous operation, with a boolean indicating success or failure.</returns>
    Task<bool> ResetPasswordAsync(string token, string newPassword);

    /// <summary>
    /// Confirms a user's email address using the confirmation token.
    /// </summary>
    /// <param name="token">The email confirmation token received via email.</param>
    /// <returns>A task representing the asynchronous operation, with a boolean indicating success or failure.</returns>
    Task<bool> ConfirmEmailAsync(string token);

    /// <summary>
    /// Resends the email confirmation link to the user's email address.
    /// </summary>
    /// <param name="email">The email address to send the confirmation link to.</param>
    /// <returns>A task representing the asynchronous operation, with a boolean indicating success or failure.</returns>
    Task<bool> ResendConfirmationEmailAsync(string email);
}