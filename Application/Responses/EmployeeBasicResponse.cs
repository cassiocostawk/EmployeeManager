namespace Application.Responses
{
    public class EmployeeBasicResponse
    {
        public Guid Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string DocNumber { get; set; }
        public required string BirthDate { get; set; }
        public required string Role { get; set; }
        public Guid? ManagerId { get; set; }

        public virtual EmployeeResponse? Manager { get; set; }
    }
}
