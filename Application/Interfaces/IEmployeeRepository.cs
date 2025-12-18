using Application.Responses;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<PagedResponse<Employee>> GetPagedAsync(
            int page,
            int pageSize,
            CancellationToken cancellationToken
        );
        Task<Domain.Entities.Employee> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task CreateAsync(Employee employee, CancellationToken cancellationToken);
        Task UpdateAsync(Employee employee, CancellationToken cancellationToken);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken);
        Task<bool> DocumentExistsAsync(string documentNumber, CancellationToken cancellationToken);
    }
}
