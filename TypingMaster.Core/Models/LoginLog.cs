namespace TypingMaster.Core.Models;

public class LoginLog
{
    public int Id { get; set; }

    public int AccountId { get; set; }

    public string? IpAddress { get; set; }

    public string? UserAgent { get; set; }

    public DateTime LoginTime { get; set; }

    public bool IsSuccessful { get; set; }

    public string? FailureReason { get; set; }
}