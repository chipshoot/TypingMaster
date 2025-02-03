namespace TypingMaster.Server.Dto;

public class UserProfileDto
{
    public int Id { get; set; }
    public int AccountNumberId { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Title { get; set; }
}