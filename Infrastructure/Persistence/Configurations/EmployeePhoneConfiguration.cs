using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class EmployeePhoneConfiguration : IEntityTypeConfiguration<EmployeePhone>
    {
        public void Configure(EntityTypeBuilder<EmployeePhone> builder)
        {
            builder.ToTable("EmployeePhones");

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

            builder.Property(x => x.EmployeeId)
                .IsRequired();

            builder.Property(x => x.PhoneNumber)
                .IsRequired()
                .HasMaxLength(25);

            #endregion

            #region Relationships
            builder.HasOne(x => x.Employee)
                .WithMany(x => x.Phones)
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            #endregion
        }
    }
}
