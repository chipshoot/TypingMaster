namespace TypingMaster.Business.Contract;

/// <summary>
/// Interface to handle user authentication and authorization.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Logs in a user with the provided username and password.
    /// </summary>
    /// <param name="username">The username of the user.</param>
    /// <param name="password">The password of the user.</param>
    /// <returns>A task representing the asynchronous operation, with a boolean result indicating success or failure.</returns>
    Task<bool> LoginAsync(string username, string password);

    /// <summary>
    /// Logs out the currently logged-in user.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task LogoutAsync();

    /// <summary>
    /// Registers a new user with the provided username and password.
    /// </summary>
    /// <param name="username">The username of the new user.</param>
    /// <param name="password">The password of the new user.</param>
    /// <returns>A task representing the asynchronous operation, with a boolean result indicating success or failure.</returns>
    Task<bool> RegisterAsync(string username, string password);

    /// <summary>
    /// Authenticates with the IDP and obtains a JWT.
    /// </summary>
    /// <param name="username">The username of the user.</param>
    /// <param name="password">The password of the user.</param>
    /// <returns>A task representing the asynchronous operation, with a string result containing the JWT.</returns>
    Task<string> AuthenticateWithIdpAsync(string username, string password);
}