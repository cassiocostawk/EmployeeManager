using Domain.Enums;

namespace Application.Requests
{
    public class CreateEmployeeRequest
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string DocNumber { get; set; }
        public required string Password { get; set; }
        public required string BirthDate { get; set; }
        public EnumEmployeeRoles Role { get; set; }
        public Guid? ManagerId { get; set; }
        public IEnumerable<EmployeePhoneRequest>? Phones { get; set; }
    }
}
