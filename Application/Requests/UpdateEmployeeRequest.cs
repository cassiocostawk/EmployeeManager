using Domain.Enums;

namespace Application.Requests
{
    public class UpdateEmployeeRequest
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Password { get; set; }
        public string? BirthDate { get; set; }
        public bool? Active { get; set; }
        public EnumEmployeeRoles? Role { get; set; }
        public IEnumerable<EmployeePhoneRequest>? Phones { get; set; }
    }
}
