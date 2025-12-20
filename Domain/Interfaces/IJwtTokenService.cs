using Domain.Common;
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IJwtTokenService
    {
        TokenResult Generate(Employee employee);
    }
}
