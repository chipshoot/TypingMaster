namespace TypingMaster.Server.Dto;

public class AccountRegistrationDto
{
    public int Id { get; set; }
    public string AccountNumber { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public UserProfileDto UserProfile { get; set; }
}