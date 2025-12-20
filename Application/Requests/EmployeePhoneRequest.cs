namespace Application.Requests
{
    public class EmployeePhoneRequest
    {
        public Guid? Id { get; set; }
        public required string PhoneNumber { get; set; }
    }
}
