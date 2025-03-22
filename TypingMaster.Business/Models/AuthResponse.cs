namespace TypingMaster.Business.Models
{
    public class AuthResponse
    {
        public bool Success { get; set; }

        public string? Message { get; set; } = null!;

        public string? Token { get; set; }

        public string? RefreshToken { get; set; }

        public DateTime? Expiration { get; set; }

        public int AccountId { get; set; }

        public string? AccountName { get; set; }
    }
}