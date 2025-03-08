namespace TypingMaster.Business.Models;

public class UserProfile
{
    public string? FirstName { get; set; }

    public string LastName { get; set; } = null!;

    public string? Title { get; set; }

    public string? PhoneNumber { get; set; }

    public string? AvatarUrl { get; set; }
}