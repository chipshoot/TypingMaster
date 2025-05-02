namespace TypingMaster.Server.Models;

public class RefreshTokenRequest
{
    public string Email {get; set; } = string.Empty;

    public string Token { get; set; } = string.Empty;

    public string RefreshToken { get; set; } = string.Empty;
}