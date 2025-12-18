using Domain.Enums;

namespace Domain.Entities
{
    public class Employee : BaseEntity
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string DocNumber { get; set; }
        public required string Password { get; set; }
        public required DateOnly BirthDate { get; set; }
        public required EnumEmployeeRoles Role { get; set; }
        public Guid? ManagerId { get; set; }

        public virtual ICollection<EmployeePhone>? Phones { get; set; }
        public virtual Employee? Manager { get; set; }
    }
}
