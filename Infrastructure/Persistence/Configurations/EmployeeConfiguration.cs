using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.ToTable("Employees");

            #region Props
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.CreatedAt)
                .HasDefaultValueSql("GETDATE()")
                .IsRequired();

            builder.Property(x => x.Active)
                .HasDefaultValue(true)
                .IsRequired();

            builder.Property(x => x.FirstName)
                .IsRequired()
                .HasMaxLength(60);

            builder.Property(x => x.LastName)
                .IsRequired()
                .HasMaxLength(60);

            builder.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.DocNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(x => x.Password)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.BirthDate)
                .HasConversion(
                    x => x.ToDateTime(TimeOnly.MinValue),
                    x => DateOnly.FromDateTime(x)
                )
                .IsRequired();

            builder.Property(x => x.Role)
                .IsRequired();

            #endregion

            #region Unique Constraints
            builder.HasIndex(x => x.Email).IsUnique();
            builder.HasIndex(x => x.DocNumber).IsUnique();
            #endregion

            #region Relationships
            builder.HasOne(x => x.Manager)
                .WithMany()
                .HasForeignKey(x => x.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Phones)
                .WithOne(x => x.Employee)
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            #endregion
        }
    }
}
