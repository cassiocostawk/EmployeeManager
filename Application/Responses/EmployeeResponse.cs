namespace Application.Responses
{
    public class EmployeeResponse
    {
        public Guid Id { get; set; }
        public bool Active { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string DocNumber { get; set; }
        public required string BirthDate { get; set; }
        public required string Role { get; set; }
        public Guid? ManagerId { get; set; }

        public virtual ICollection<EmployeePhoneResponse>? Phones { get; set; }
    }
}
