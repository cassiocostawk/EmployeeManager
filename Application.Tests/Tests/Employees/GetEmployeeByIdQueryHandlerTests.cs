using Application.Queries;
using Application.Tests.Tests.Mocks;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;

namespace Application.Tests.Tests.Employees
{
    public class GetEmployeeByIdQueryHandlerTests
    {
        #region Successful Retrieval Tests

        [Fact]
        public async Task Should_Return_Employee_When_Exists()
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

            var handler = new GetEmployeeByIdQueryHandler(mapper, repository);
            var query = new GetEmployeeByIdQuery(employeeId);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(employeeId, result.Id);
            Assert.Equal("Employee", result.FirstName);
            Assert.Equal("User", result.LastName);
            Assert.Equal("employee@test.com", result.Email);
        }

        [Fact]
        public async Task Should_Return_Employee_With_Phones()
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
                CreatedAt = DateTime.UtcNow,
                Phones = new List<EmployeePhone>
                {
                    new EmployeePhone
                    {
                        Id = Guid.NewGuid(),
                        PhoneNumber = "111111111",
                        EmployeeId = employeeId
                    }
                }
            }, CancellationToken.None);

            var handler = new GetEmployeeByIdQueryHandler(mapper, repository);
            var query = new GetEmployeeByIdQuery(employeeId);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Phones);
            Assert.Single(result.Phones);
            Assert.Equal("111111111", result.Phones.First().PhoneNumber);
        }

        #endregion

        #region Validation Tests

        [Fact]
        public async Task Should_Throw_NotFoundException_When_Employee_Does_Not_Exist()
        {
            // Arrange
            var mapper = MapperMock.Create();
            var repository = new EmployeeRepositoryMock();
            var handler = new GetEmployeeByIdQueryHandler(mapper, repository);
            var query = new GetEmployeeByIdQuery(Guid.NewGuid());

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Should_Throw_ValidationException_If_Id_Is_Empty()
        {
            // Arrange
            var mapper = MapperMock.Create();
            var repository = new EmployeeRepositoryMock();
            var handler = new GetEmployeeByIdQueryHandler(mapper, repository);
            var query = new GetEmployeeByIdQuery(Guid.Empty);

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() =>
                handler.Handle(query, CancellationToken.None));
        }

        #endregion
    }
}