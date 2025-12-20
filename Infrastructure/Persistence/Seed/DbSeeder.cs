using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Seed
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext dbContext, IPasswordHasher passwordHasher)
        {
            if (await dbContext.Employees.AnyAsync())
                return;

            var mainEmployee = new Employee
            {
                FirstName = "Admin",
                LastName = "User",
                Email = "admin@email.com",
                Role = Domain.Enums.EnumEmployeeRoles.Director,
                DocNumber = "00100100101",
                Password = passwordHasher.Hash("Admin@123"),
                BirthDate = new DateOnly(1990, 2, 1),
                Phones =
                [
                    new EmployeePhone
                    {
                        PhoneNumber = "+1234567890"
                    }
                ],
            };

            dbContext.Employees.Add(mainEmployee);
            await dbContext.SaveChangesAsync();
        }
    }
}
