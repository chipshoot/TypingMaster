namespace TypingMaster.Server.Models;

public class ConfirmRegistrationRequest
{
    public string UserName { get; set; } = string.Empty;
    public string ConfirmationCode { get; set; } = string.Empty;
}