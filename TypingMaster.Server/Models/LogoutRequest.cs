namespace TypingMaster.Server.Models;

public class LogoutRequest
{
    public int AccountId { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
}