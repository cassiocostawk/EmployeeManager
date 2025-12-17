namespace Domain.Entities
{
    public class EmployeePhone
    {
        public Guid Id { get; set; }
        public required Guid EmployeeId { get; set; }
        public required string PhoneNumber { get; set; }

        public virtual Employee? Employee { get; set; }
    }
}
