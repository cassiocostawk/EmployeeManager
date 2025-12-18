namespace Domain.Entities
{
    public class EmployeePhone : BaseEntity
    {
        public Guid EmployeeId { get; set; }
        public required string PhoneNumber { get; set; }

        public virtual Employee? Employee { get; set; }
    }
}
