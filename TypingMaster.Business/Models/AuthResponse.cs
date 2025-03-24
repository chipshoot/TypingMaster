namespace TypingMaster.Business.Models
{
    public class AuthResponse
    {
        public bool Success { get; set; }

        public string Message { get; set; } = string.Empty;

        public string Token { get; set; } = string.Empty;

        public string RefreshToken { get; set; } = string.Empty;

        public DateTime? Expiration { get; set; }

        public int AccountId { get; set; }

        public string AccountName { get; set; } = string.Empty;
    }
}