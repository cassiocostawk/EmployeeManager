namespace Application.Responses
{
    public class EmployeePhoneResponse
    {
        public Guid Id { get; set; }
        public required string PhoneNumber { get; set; }
    }
}
