using Application.Commands;
using Application.Tests.Tests.Mocks;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;

namespace Application.Tests.Tests.Employees
{
    public class DeleteEmployeeCommandHandlerTests
    {
        #region Successful Deletion Tests

        [Fact]
        public async Task Should_Delete_Employee_Successfully()
        {
            // Arrange
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

            var handler = new DeleteEmployeeCommandHandler(
                repository,
                new CurrentUserMock { Role = EnumEmployeeRoles.Director }
            );

            var command = new DeleteEmployeeCommand(employeeId);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            var employee = repository.GetFirst();
            Assert.Null(employee); // Employee should be deleted
        }

        [Fact]
        public async Task Should_Allow_Deleting_Employee_With_Same_Role()
        {
            // Arrange
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

            var handler = new DeleteEmployeeCommandHandler(
                repository,
                new CurrentUserMock { Role = EnumEmployeeRoles.Employee }
            );

            var command = new DeleteEmployeeCommand(employeeId);

            // Act - Não deve lançar exceção
            await handler.Handle(command, CancellationToken.None);

            // Assert
            var employee = repository.GetFirst();
            Assert.Null(employee); // Employee should be deleted successfully
        }

        #endregion

        #region Role Hierarchy Tests

        [Fact]
        public async Task Should_Not_Allow_Deleting_Employee_With_Higher_Role()
        {
            // Arrange
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

            var handler = new DeleteEmployeeCommandHandler(
                repository,
                new CurrentUserMock { Role = EnumEmployeeRoles.Employee }
            );

            var command = new DeleteEmployeeCommand(employeeId);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BusinessRuleException>(() =>
                handler.Handle(command, CancellationToken.None));

            Assert.Equal("Unauthorized to remove Employee with higher role level.", exception.Message);
        }

        [Fact]
        public async Task Should_Allow_Director_To_Delete_Leader()
        {
            // Arrange
            var repository = new EmployeeRepositoryMock();
            var employeeId = Guid.NewGuid();

            await repository.CreateAsync(new Employee
            {
                Id = employeeId,
                FirstName = "Leader",
                LastName = "User",
                Email = "leader@test.com",
                DocNumber = "123",
                Password = "hashed_password",
                BirthDate = new DateOnly(1990, 1, 1),
                Role = EnumEmployeeRoles.Leader,
                Active = true,
                CreatedAt = DateTime.UtcNow
            }, CancellationToken.None);

            var handler = new DeleteEmployeeCommandHandler(
                repository,
                new CurrentUserMock { Role = EnumEmployeeRoles.Director }
            );

            var command = new DeleteEmployeeCommand(employeeId);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            var employee = repository.GetFirst();
            Assert.Null(employee); // Should be deleted successfully
        }

        [Fact]
        public async Task Should_Allow_Leader_To_Delete_Employee()
        {
            // Arrange
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

            var handler = new DeleteEmployeeCommandHandler(
                repository,
                new CurrentUserMock { Role = EnumEmployeeRoles.Leader }
            );

            var command = new DeleteEmployeeCommand(employeeId);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            var employee = repository.GetFirst();
            Assert.Null(employee); // Should be deleted successfully
        }

        #endregion

        #region Validation Tests

        [Fact]
        public async Task Should_Throw_ValidationException_If_Id_Is_Empty()
        {
            // Arrange
            var handler = new DeleteEmployeeCommandHandler(
                new EmployeeRepositoryMock(),
                new CurrentUserMock { Role = EnumEmployeeRoles.Director }
            );

            var command = new DeleteEmployeeCommand(Guid.Empty);

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() =>
                handler.Handle(command, CancellationToken.None));
        }

        #endregion
    }
}