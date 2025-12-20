namespace Domain.Common
{
    public class TokenResult
    {
        public string AccessToken { get; set; }
        public DateTime ExpiresAt { get; set; }

        public TokenResult(string accessToken, DateTime expiresAt)
        {
            AccessToken = accessToken;
            ExpiresAt = expiresAt;
        }
    }
}
