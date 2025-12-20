using Application.Queries;
using Application.Tests.Tests.Mocks;
using Domain.Entities;
using Domain.Enums;

namespace Application.Tests.Tests.Employees
{
    public class GetEmployeePagedQueryHandlerTests
    {
        #region Pagination Tests

        [Fact]
        public async Task Should_Return_Correct_Page_Size()
        {
            // Arrange
            var mapper = MapperMock.Create();
            var repository = new EmployeeRepositoryMock();

            // Create 15 employees
            for (int i = 1; i <= 15; i++)
            {
                await repository.CreateAsync(new Employee
                {
                    Id = Guid.NewGuid(),
                    FirstName = $"Employee{i}",
                    LastName = "User",
                    Email = $"employee{i}@test.com",
                    DocNumber = $"DOC{i}",
                    Password = "hashed_password",
                    BirthDate = new DateOnly(1990, 1, 1),
                    Role = EnumEmployeeRoles.Employee,
                    Active = true,
                    CreatedAt = DateTime.UtcNow
                }, CancellationToken.None);
            }

            var handler = new GetEmployeePagedQueryHandler(mapper, repository);
            var query = new GetEmployeePagedQuery { Page = 1, PageSize = 10 };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(10, result.Items!.Count);
            Assert.Equal(15, result.TotalCount);
            Assert.Equal(10, result.PageSize);
            Assert.Equal(1, result.CurrentPage);
        }

        [Fact]
        public async Task Should_Return_Second_Page_Correctly()
        {
            // Arrange
            var mapper = MapperMock.Create();
            var repository = new EmployeeRepositoryMock();

            // Create 15 employees
            for (int i = 1; i <= 15; i++)
            {
                await repository.CreateAsync(new Employee
                {
                    Id = Guid.NewGuid(),
                    FirstName = $"Employee{i}",
                    LastName = "User",
                    Email = $"employee{i}@test.com",
                    DocNumber = $"DOC{i}",
                    Password = "hashed_password",
                    BirthDate = new DateOnly(1990, 1, 1),
                    Role = EnumEmployeeRoles.Employee,
                    Active = true,
                    CreatedAt = DateTime.UtcNow
                }, CancellationToken.None);
            }

            var handler = new GetEmployeePagedQueryHandler(mapper, repository);
            var query = new GetEmployeePagedQuery { Page = 2, PageSize = 10 };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(5, result.Items!.Count); // Remaining items on page 2
            Assert.Equal(15, result.TotalCount);
            Assert.Equal(10, result.PageSize);
            Assert.Equal(2, result.CurrentPage);
        }

        [Fact]
        public async Task Should_Return_All_Items_When_PageSize_Greater_Than_Total()
        {
            // Arrange
            var mapper = MapperMock.Create();
            var repository = new EmployeeRepositoryMock();

            // Create 5 employees
            for (int i = 1; i <= 5; i++)
            {
                await repository.CreateAsync(new Employee
                {
                    Id = Guid.NewGuid(),
                    FirstName = $"Employee{i}",
                    LastName = "User",
                    Email = $"employee{i}@test.com",
                    DocNumber = $"DOC{i}",
                    Password = "hashed_password",
                    BirthDate = new DateOnly(1990, 1, 1),
                    Role = EnumEmployeeRoles.Employee,
                    Active = true,
                    CreatedAt = DateTime.UtcNow
                }, CancellationToken.None);
            }

            var handler = new GetEmployeePagedQueryHandler(mapper, repository);
            var query = new GetEmployeePagedQuery { Page = 1, PageSize = 20 };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(5, result.Items!.Count);
            Assert.Equal(5, result.TotalCount);
        }

        #endregion

        #region Default Values Tests

        [Fact]
        public async Task Should_Use_Default_Values_When_Not_Specified()
        {
            // Arrange
            var mapper = MapperMock.Create();
            var repository = new EmployeeRepositoryMock();

            // Create 25 employees
            for (int i = 1; i <= 25; i++)
            {
                await repository.CreateAsync(new Employee
                {
                    Id = Guid.NewGuid(),
                    FirstName = $"Employee{i}",
                    LastName = "User",
                    Email = $"employee{i}@test.com",
                    DocNumber = $"DOC{i}",
                    Password = "hashed_password",
                    BirthDate = new DateOnly(1990, 1, 1),
                    Role = EnumEmployeeRoles.Employee,
                    Active = true,
                    CreatedAt = DateTime.UtcNow
                }, CancellationToken.None);
            }

            var handler = new GetEmployeePagedQueryHandler(mapper, repository);
            var query = new GetEmployeePagedQuery(); // Uses default Page = 1, PageSize = 10

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(10, result.Items!.Count); // Default PageSize
            Assert.Equal(25, result.TotalCount);
            Assert.Equal(10, result.PageSize);
            Assert.Equal(1, result.CurrentPage); // Default Page
        }

        #endregion

        #region Empty Result Tests

        [Fact]
        public async Task Should_Return_Empty_List_When_No_Employees()
        {
            // Arrange
            var mapper = MapperMock.Create();
            var repository = new EmployeeRepositoryMock();
            var handler = new GetEmployeePagedQueryHandler(mapper, repository);
            var query = new GetEmployeePagedQuery { Page = 1, PageSize = 10 };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Items!);
            Assert.Equal(0, result.TotalCount);
        }

        #endregion
    }
}