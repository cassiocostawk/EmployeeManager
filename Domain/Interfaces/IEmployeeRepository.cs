using Domain.Common;
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<PagedResult<Employee>> GetPagedAsync(
            int page,
            int pageSize,
            CancellationToken cancellationToken
        );
        Task<Employee> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task CreateAsync(Employee employee, CancellationToken cancellationToken);
        Task UpdateAsync(Employee employee, CancellationToken cancellationToken);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken);
        Task<bool> DocumentExistsAsync(string documentNumber, CancellationToken cancellationToken);
    }
}
