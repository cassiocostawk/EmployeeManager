using Domain.Common;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Tests.Tests.Mocks
{
    public class EmployeeRepositoryMock : IEmployeeRepository
    {
        private readonly List<Employee> _employees = [];

        public async Task<Employee?> GetByDocNumberAsync(string documentNumber, CancellationToken cancellationToken)
        {
            return await Task.FromResult(_employees.FirstOrDefault(e => e.DocNumber == documentNumber));
        }

        public async Task<Employee?> GetByEmailAsync(string email, CancellationToken cancellationToken)
        {
            return await Task.FromResult(_employees.FirstOrDefault(e => e.Email == email));
        }

        public async Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await Task.FromResult(_employees.FirstOrDefault(e => e.Id == id));
        }

        public async Task<PagedResult<Employee>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
            return await Task.FromResult(new PagedResult<Employee>
            {
                Items = _employees.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                TotalCount = _employees.Count,
                PageSize = pageSize,
                CurrentPage = page
            });
        }

        public Task CreateAsync(Employee employee, CancellationToken cancellationToken)
        {
            _employees.Add(employee);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            _employees.RemoveAll(e => e.Id == id);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Employee employee, CancellationToken cancellationToken)
        {
            var index = _employees.FindIndex(e => e.Id == employee.Id);
            if (index != -1)
            {
                _employees[index] = employee;
            }
            return Task.CompletedTask;
        }

        public Employee? GetFirst() => _employees.FirstOrDefault();
        
        public List<Employee> GetAll() => _employees.ToList();
        
        public void Clear() => _employees.Clear();
    }
}
