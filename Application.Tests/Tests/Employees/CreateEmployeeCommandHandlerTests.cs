using Application.Commands;
using Application.Requests;
using Application.Tests.Tests.Mocks;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;

namespace Application.Tests.Tests.Employees
{
    public class CreateEmployeeCommandHandlerTests
    {
        [Fact]
        public async Task Should_create_employee_with_hashed_password()
        {
            var mapper = MapperMock.Create();
            var repository = new EmployeeRepositoryMock();
            var currentUser = new CurrentUserMock
            {
                Role = EnumEmployeeRoles.Director
            };
            var passwordHasher = new PasswordHasherMock();

            var handler = new CreateEmployeeCommandHandler(
                mapper,
                repository,
                passwordHasher,
                currentUser
            );

            var command = new CreateEmployeeCommand(new CreateEmployeeRequest
            {
                FirstName = "Employee",
                LastName = "User",
                Email = "employee@test.com",
                DocNumber = "123",
                Password = "123456",
                Role = EnumEmployeeRoles.Employee,
                BirthDate = "1990/01/01"
            });

            await handler.Handle(command, CancellationToken.None);

            var employee = repository.GetFirst();

            Assert.NotNull(employee);
            Assert.Equal("hashed_123456", employee!.Password);
        }

        [Fact]
        public async Task Should_throw_if_document_already_exists()
        {
            var mapper = MapperMock.Create();
            var repository = new EmployeeRepositoryMock();
            await repository.CreateAsync(new Employee
            {
                Id = Guid.NewGuid(),
                DocNumber = "123",
                FirstName = "Existing",
                LastName = "User",
                Email = "existing@test.com",
                Password = "hashed_password",
                BirthDate = new DateOnly(1990, 1, 1),
                Role = EnumEmployeeRoles.Employee,
                Active = true,
                CreatedAt = DateTime.UtcNow
            }, CancellationToken.None);

            var handler = new CreateEmployeeCommandHandler(
                mapper,
                repository,
                new PasswordHasherMock(),
                new CurrentUserMock { Role = EnumEmployeeRoles.Director }
            );

            var command = new CreateEmployeeCommand(new CreateEmployeeRequest
            {
                FirstName = "New",
                LastName = "User",
                Email = "new@test.com",
                DocNumber = "123",
                Password = "123456",
                Role = EnumEmployeeRoles.Employee,
                BirthDate = "1990/01/01"
            });

            await Assert.ThrowsAsync<BusinessRuleException>(() =>
                handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Should_not_allow_creating_employee_with_higher_role()
        {
            var mapper = MapperMock.Create();
            var handler = new CreateEmployeeCommandHandler(
                mapper,
                new EmployeeRepositoryMock(),
                new PasswordHasherMock(),
                new CurrentUserMock { Role = EnumEmployeeRoles.Employee }
            );

            var command = new CreateEmployeeCommand(new CreateEmployeeRequest
            {
                FirstName = "Director",
                LastName = "User",
                Email = "director@test.com",
                DocNumber = "123",
                Role = EnumEmployeeRoles.Director,
                Password = "123456",
                BirthDate = "1990/01/01"
            });

            var exception = await Assert.ThrowsAsync<BusinessRuleException>(() =>
                handler.Handle(command, CancellationToken.None));

            Assert.Equal("Unauthorized to create Employee with higher role level.", exception.Message);
        }

        [Fact]
        public async Task Should_Throw_BusinessRuleException_If_Employee_Creating_Same_Role()
        {
            var mapper = MapperMock.Create();
            var currentUser = new CurrentUserMock { Role = EnumEmployeeRoles.Employee };
            var handler = new CreateEmployeeCommandHandler(
                mapper,
                new EmployeeRepositoryMock(),
                new PasswordHasherMock(),
                currentUser
            );

            var command = new CreateEmployeeCommand(new CreateEmployeeRequest
            {
                FirstName = "Employee",
                LastName = "User",
                Email = "employee@test.com",
                DocNumber = "123",
                Role = EnumEmployeeRoles.Employee,
                Password = "123456",
                BirthDate = "1990/01/01"
            });

            await Assert.ThrowsAsync<BusinessRuleException>(() =>
                handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Should_Throw_BusinessRuleException_If_Email_Already_Exists()
        {
            var mapper = MapperMock.Create();
            var repository = new EmployeeRepositoryMock();
            
            await repository.CreateAsync(new Employee
            {
                Id = Guid.NewGuid(),
                DocNumber = "456",
                FirstName = "Existing",
                LastName = "User",
                Email = "employee@test.com",
                Password = "hashed_password",
                BirthDate = new DateOnly(1990, 1, 1),
                Role = EnumEmployeeRoles.Employee,
                Active = true,
                CreatedAt = DateTime.UtcNow
            }, CancellationToken.None);

            var handler = new CreateEmployeeCommandHandler(
                mapper,
                repository,
                new PasswordHasherMock(),
                new CurrentUserMock { Role = EnumEmployeeRoles.Director }
            );

            var command = new CreateEmployeeCommand(new CreateEmployeeRequest
            {
                FirstName = "New",
                LastName = "User",
                Email = "employee@test.com",
                DocNumber = "123",
                Password = "123456",
                Role = EnumEmployeeRoles.Employee,
                BirthDate = "1990/01/01"
            });

            var exception = await Assert.ThrowsAsync<BusinessRuleException>(() =>
                handler.Handle(command, CancellationToken.None));
            
            Assert.Equal("Email already exists", exception.Message);
        }
    }
}
