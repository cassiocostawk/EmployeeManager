using Domain.Common;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext _dbContext;

        public EmployeeRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Employee?> GetByDocNumberAsync(string documentNumber, CancellationToken cancellationToken)
        {
            var data = await _dbContext.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.DocNumber == documentNumber, cancellationToken);

            return data;
        }

        public async Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var data = await _dbContext.Employees
                .AsNoTracking()
                .Include(x => x.Phones)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            return data;
        }

        public async Task<PagedResult<Employee>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
            var data = await _dbContext.Employees
                .AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var totalCount = await _dbContext.Employees.CountAsync(cancellationToken);

            return new PagedResult<Employee>
            {
                Items = data,
                TotalCount = totalCount,
                PageSize = pageSize,
                CurrentPage = page
            };
        }
        public async Task CreateAsync(Employee employee, CancellationToken cancellationToken)
        {
            await _dbContext.Employees.AddAsync(employee, cancellationToken);
            var rowsAffected = await _dbContext.SaveChangesAsync(cancellationToken);

            if (rowsAffected == 0)
                throw new InvalidOperationException("Employee not found."); // TODO: Exception Middleware stage - implement on Middleware
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var rowsAffected = await _dbContext.Employees
                .Where(x => x.Id == id)
                .ExecuteDeleteAsync(cancellationToken);

            if (rowsAffected == 0)
                throw new KeyNotFoundException("Employee not found."); // TODO: Exception Middleware stage - implement on Middleware
        }
        public async Task UpdateAsync(Employee employee, CancellationToken cancellationToken)
        {
            _dbContext.Update(employee);
            var rowsAffected = await _dbContext.SaveChangesAsync(cancellationToken);

            if (rowsAffected == 0)
                throw new KeyNotFoundException("Employee not found."); // TODO: Exception Middleware stage - implement on Middleware
        }
    }
}
