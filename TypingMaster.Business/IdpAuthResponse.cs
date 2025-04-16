namespace TypingMaster.Business;

public class IdpAuthResponse
{
    public bool Success { get; set; }

    public string AccessToken { get; set; } = string.Empty;

    public string TokenType { get; set; } = string.Empty;

    public int ExpiresIn { get; set; }

    public string RefreshToken { get; set; } = string.Empty;

    public string Scope { get; set; } = string.Empty;

    public string? Message { get; set; }

    public string? ChallengeName { get; set; }

    public string? Session { get; set; }
}