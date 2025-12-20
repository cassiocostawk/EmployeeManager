namespace Application.Responses
{
    public class LoginResponse
    {
        public required EmployeeBasicResponse CurrentUser { get; set; }
        public required string AccessToken { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
