using System.Security.Cryptography;
using System.Text;

namespace TypingMaster.Business;

/// <summary>
/// Provides password hashing and verification functionality.
/// Can be replaced with Identity's password hasher in the future IdP integration.
/// </summary>
public class PasswordHasher
{
    private const int SaltSize = 16; // 128 bits
    private const int KeySize = 32; // 256 bits
    private const int Iterations = 10000;
    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;
    private const char Delimiter = ':';

    /// <summary>
    /// Generates a random salt and hashes the password
    /// </summary>
    /// <param name="password">Plain text password</param>
    /// <returns>Tuple with (hash, salt)</returns>
    public static (string hash, string salt) HashPassword(string password)
    {
        // Generate a random salt
        var salt = RandomNumberGenerator.GetBytes(SaltSize);

        // Hash the password with the salt
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            Iterations,
            Algorithm,
            KeySize);

        // Convert to strings
        var saltString = Convert.ToBase64String(salt);
        var hashString = Convert.ToBase64String(hash);

        return (hashString, saltString);
    }

    /// <summary>
    /// Verifies a password against a stored hash and salt
    /// </summary>
    /// <param name="password">Plain text password to verify</param>
    /// <param name="hash">Stored password hash</param>
    /// <param name="salt">Stored salt</param>
    /// <returns>True if password matches, false otherwise</returns>
    public static bool VerifyPassword(string password, string hash, string salt)
    {
        // Convert salt from string to bytes
        var saltBytes = Convert.FromBase64String(salt);

        // Hash the input password with the same salt
        var hashToCheck = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            saltBytes,
            Iterations,
            Algorithm,
            KeySize);

        var hashToCheckString = Convert.ToBase64String(hashToCheck);

        // Compare the hashes
        return hash == hashToCheckString;
    }

    /// <summary>
    /// Generates a random secure token (for password reset, etc.)
    /// </summary>
    /// <param name="length">Length of token in bytes (will be longer as base64 string)</param>
    /// <returns>Secure random token as base64 string</returns>
    public static string GenerateSecureToken(int length = 32)
    {
        var tokenBytes = RandomNumberGenerator.GetBytes(length);
        return Convert.ToBase64String(tokenBytes);
    }
}