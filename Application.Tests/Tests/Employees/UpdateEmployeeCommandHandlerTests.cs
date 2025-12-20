using Application.Commands;
using Application.Requests;
using Application.Tests.Tests.Mocks;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;

namespace Application.Tests.Tests.Employees
{
    public class UpdateEmployeeCommandHandlerTests
    {
        #region Successful Update Tests

        [Fact]
        public async Task Should_Update_Employee_Successfully()
        {
            // Arrange
            var mapper = MapperMock.Create();
            var repository = new EmployeeRepositoryMock();
            var employeeId = Guid.NewGuid();

            await repository.CreateAsync(new Employee
            {
                Id = employeeId,
                FirstName = "Old",
                LastName = "Name",
                Email = "old@test.com",
                DocNumber = "123",
                Password = "hashed_oldpassword",
                BirthDate = new DateOnly(1990, 1, 1),
                Role = EnumEmployeeRoles.Employee,
                Active = true,
                CreatedAt = DateTime.UtcNow
            }, CancellationToken.None);

            var handler = new UpdateEmployeeCommandHandler(
                mapper,
                repository,
                new PasswordHasherMock(),
                new CurrentUserMock { Role = EnumEmployeeRoles.Director }
            );

            var command = new UpdateEmployeeCommand(employeeId, new UpdateEmployeeRequest
            {
                FirstName = "Updated",
                LastName = "User",
                Password = "newpassword"
            });

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            var employee = repository.GetFirst();
            Assert.NotNull(employee);
            Assert.Equal("Updated", employee!.FirstName);
            Assert.Equal("User", employee.LastName);
            Assert.Equal("hashed_newpassword", employee.Password);
        }

        [Fact]
        public async Task Should_Allow_Updating_Employee_With_Same_Role()
        {
            // Arrange
            var mapper = MapperMock.Create();
            var repository = new EmployeeRepositoryMock();
            var employeeId = Guid.NewGuid();

            await repository.CreateAsync(new Employee
            {
                Id = employeeId,
                FirstName = "Employee",
                LastName = "User",
                Email = "employee@test.com",
                DocNumber = "123",
                Password = "hashed_password",
                BirthDate = new DateOnly(1990, 1, 1),
                Role = EnumEmployeeRoles.Employee,
                Active = true,
                CreatedAt = DateTime.UtcNow
            }, CancellationToken.None);

            var handler = new UpdateEmployeeCommandHandler(
                mapper,
                repository,
                new PasswordHasherMock(),
                new CurrentUserMock { Role = EnumEmployeeRoles.Employee }
            );

            var command = new UpdateEmployeeCommand(employeeId, new UpdateEmployeeRequest
            {
                FirstName = "Updated"
            });

            // Act - Não deve lançar exceção
            await handler.Handle(command, CancellationToken.None);

            // Assert
            var employee = repository.GetFirst();
            Assert.NotNull(employee);
            Assert.Equal("Updated", employee!.FirstName);
        }

        [Fact]
        public async Task Should_Update_Employee_Phones()
        {
            // Arrange
            var mapper = MapperMock.Create();
            var repository = new EmployeeRepositoryMock();
            var employeeId = Guid.NewGuid();
            var phoneId = Guid.NewGuid();

            await repository.CreateAsync(new Employee
            {
                Id = employeeId,
                FirstName = "Employee",
                LastName = "User",
                Email = "employee@test.com",
                DocNumber = "123",
                Password = "hashed_password",
                BirthDate = new DateOnly(1990, 1, 1),
                Role = EnumEmployeeRoles.Employee,
                Active = true,
                CreatedAt = DateTime.UtcNow,
                Phones = new List<EmployeePhone>
                {
                    new EmployeePhone
                    {
                        Id = phoneId,
                        PhoneNumber = "111111111",
                        EmployeeId = employeeId
                    }
                }
            }, CancellationToken.None);

            var handler = new UpdateEmployeeCommandHandler(
                mapper,
                repository,
                new PasswordHasherMock(),
                new CurrentUserMock { Role = EnumEmployeeRoles.Director }
            );

            var command = new UpdateEmployeeCommand(employeeId, new UpdateEmployeeRequest
            {
                Phones = new List<EmployeePhoneRequest>
                {
                    new EmployeePhoneRequest
                    {
                        Id = phoneId,
                        PhoneNumber = "999999999" // Updates existing phone
                    },
                    new EmployeePhoneRequest
                    {
                        Id = null,
                        PhoneNumber = "888888888" // Adds new phone
                    }
                }
            });

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            var employee = repository.GetFirst();
            Assert.NotNull(employee);
            Assert.NotNull(employee!.Phones);
            Assert.Equal(2, employee.Phones!.Count());
            Assert.Contains(employee.Phones, p => p.PhoneNumber == "999999999");
            Assert.Contains(employee.Phones, p => p.PhoneNumber == "888888888");
        }

        #endregion

        #region Password Update Tests

        [Fact]
        public async Task Should_Hash_New_Password_When_Updating()
        {
            // Arrange
            var mapper = MapperMock.Create();
            var repository = new EmployeeRepositoryMock();
            var employeeId = Guid.NewGuid();

            await repository.CreateAsync(new Employee
            {
                Id = employeeId,
                FirstName = "Employee",
                LastName = "User",
                Email = "employee@test.com",
                DocNumber = "123",
                Password = "hashed_oldpassword",
                BirthDate = new DateOnly(1990, 1, 1),
                Role = EnumEmployeeRoles.Employee,
                Active = true,
                CreatedAt = DateTime.UtcNow
            }, CancellationToken.None);

            var handler = new UpdateEmployeeCommandHandler(
                mapper,
                repository,
                new PasswordHasherMock(),
                new CurrentUserMock { Role = EnumEmployeeRoles.Director }
            );

            var command = new UpdateEmployeeCommand(employeeId, new UpdateEmployeeRequest
            {
                Password = "newpassword123"
            });

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            var employee = repository.GetFirst();
            Assert.NotNull(employee);
            Assert.Equal("hashed_newpassword123", employee!.Password);
            Assert.NotEqual("hashed_oldpassword", employee.Password);
        }

        #endregion

        #region Validation Tests

        [Fact]
        public async Task Should_Throw_NotFoundException_If_Employee_Does_Not_Exist()
        {
            // Arrange
            var mapper = MapperMock.Create();
            var handler = new UpdateEmployeeCommandHandler(
                mapper,
                new EmployeeRepositoryMock(),
                new PasswordHasherMock(),
                new CurrentUserMock { Role = EnumEmployeeRoles.Director }
            );

            var command = new UpdateEmployeeCommand(Guid.NewGuid(), new UpdateEmployeeRequest
            {
                FirstName = "Updated"
            });

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Should_Throw_ValidationException_If_Request_Is_Null()
        {
            // Arrange
            var mapper = MapperMock.Create();
            var handler = new UpdateEmployeeCommandHandler(
                mapper,
                new EmployeeRepositoryMock(),
                new PasswordHasherMock(),
                new CurrentUserMock { Role = EnumEmployeeRoles.Director }
            );

            var command = new UpdateEmployeeCommand(Guid.NewGuid(), null!);

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() =>
                handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Should_Throw_ValidationException_If_Id_Is_Empty()
        {
            // Arrange
            var mapper = MapperMock.Create();
            var handler = new UpdateEmployeeCommandHandler(
                mapper,
                new EmployeeRepositoryMock(),
                new PasswordHasherMock(),
                new CurrentUserMock { Role = EnumEmployeeRoles.Director }
            );

            var command = new UpdateEmployeeCommand(Guid.Empty, new UpdateEmployeeRequest
            {
                FirstName = "Updated"
            });

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() =>
                handler.Handle(command, CancellationToken.None));
        }

        #endregion

        #region Role Hierarchy Tests

        [Fact]
        public async Task Should_Not_Allow_Updating_Employee_With_Higher_Role()
        {
            // Arrange
            var mapper = MapperMock.Create();
            var repository = new EmployeeRepositoryMock();
            var employeeId = Guid.NewGuid();

            await repository.CreateAsync(new Employee
            {
                Id = employeeId,
                FirstName = "Director",
                LastName = "User",
                Email = "director@test.com",
                DocNumber = "123",
                Password = "hashed_password",
                BirthDate = new DateOnly(1990, 1, 1),
                Role = EnumEmployeeRoles.Director,
                Active = true,
                CreatedAt = DateTime.UtcNow
            }, CancellationToken.None);

            var handler = new UpdateEmployeeCommandHandler(
                mapper,
                repository,
                new PasswordHasherMock(),
                new CurrentUserMock { Role = EnumEmployeeRoles.Employee }
            );

            var command = new UpdateEmployeeCommand(employeeId, new UpdateEmployeeRequest
            {
                FirstName = "Updated",
                Role = EnumEmployeeRoles.Director
            });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BusinessRuleException>(() =>
                handler.Handle(command, CancellationToken.None));

            Assert.Equal("Unauthorized to update Employee with higher role level.", exception.Message);
        }

        #endregion
    }
}