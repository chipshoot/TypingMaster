namespace TypingMaster.Core.Models
{
    public class AuthResponse : WebServiceResponse
    {
        public string Token { get; set; } = string.Empty;

        public string RefreshToken { get; set; } = string.Empty;

        public DateTime? Expiration { get; set; }

        public int AccountId { get; set; }

        public string AccountName { get; set; } = string.Empty;
    }
}