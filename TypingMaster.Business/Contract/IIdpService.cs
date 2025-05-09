using TypingMaster.Core.Models;
using TypingMaster.Core.Utility;

namespace TypingMaster.Business.Contract;

/// <summary>
/// Interface for Identity Provider services that handle authentication and token management.
/// </summary>
public interface IIdpService
{
    /// <summary>
    /// Authenticates a user with the provided credentials.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <param name="password">The user's password.</param>
    /// <returns>A task representing the asynchronous operation, with an <see cref="IdpAuthResponse"/> containing the authentication result.</returns>
    Task<IdpAuthResponse> Authenticate(string email, string password);

    /// <summary>
    /// Responds to a new password challenge from the identity provider.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <param name="newPassword">The new password to set.</param>
    /// <param name="session">The session token from the initial authentication response.</param>
    /// <returns>A task representing the asynchronous operation, with an <see cref="IdpAuthResponse"/> containing the authentication result.</returns>
    Task<IdpAuthResponse> RespondToNewPasswordChallenge(string email, string newPassword, string session);

    /// <summary>
    /// Sets a permanent password for a user, preventing the need to change password on first login.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <param name="password">The password to set.</param>
    /// <returns>A task representing the asynchronous operation, with a boolean indicating success or failure.</returns>
    Task<bool> SetPermanentPassword(string email, string password);

    /// <summary>
    /// Refreshes an access token using a refresh token.
    /// </summary>
    /// <param name="refreshToken">The refresh token to use for generating a new access token.</param>
    /// <param name="userName">The username associated with refresh token.</param>
    /// <returns>A task representing the asynchronous operation, with an <see cref="IdpAuthResponse"/> containing the new tokens if successful.</returns>
    Task<IdpAuthResponse> RefreshToken(string refreshToken, string userName);

    /// <summary>
    /// Registers a new user with the identity provider.
    /// </summary>
    /// <param name="request">The registration request containing user information.</param>
    /// <returns>A task representing the asynchronous operation, with a boolean indicating success or failure.</returns>
    Task<bool> RegisterUser(RegisterRequest request);

    /// <summary>
    /// Confirms a user's registration using the confirmation code.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <param name="confirmationCode">The confirmation code received via email.</param>
    /// <returns>A task representing the asynchronous operation, with a boolean indicating success or failure.</returns>
    Task<bool> ConfirmRegistration(string email, string confirmationCode);

    /// <summary>
    /// Resends the confirmation code to the user via email.
    /// </summary>
    /// <param name="userName">The username of the user to whom the confirmation code will be resent.</param>
    /// <returns>A task representing the asynchronous operation, with a boolean indicating success or failure.</returns>
    Task<bool> ResendConfirmationCode(string userName);

    /// <summary>
    /// Gets or sets the result of the process, including status, error messages, and additional information.
    /// </summary>
    ProcessResult ProcessResult { get; set; }
}