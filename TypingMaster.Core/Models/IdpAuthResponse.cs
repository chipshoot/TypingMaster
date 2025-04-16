namespace TypingMaster.Core.Models;

public class IdpAuthResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public string? AccessToken { get; set; }
    public string? TokenType { get; set; }
    public int? ExpiresIn { get; set; }
    public string? RefreshToken { get; set; }
    public string? Scope { get; set; }
    public string? ChallengeName { get; set; }
    public string? Session { get; set; }
}