using TypingMaster.Client.Models;
using TypingMaster.Core.Models;
using AuthResponse = TypingMaster.Client.Models.AuthResponse;

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
    }
}