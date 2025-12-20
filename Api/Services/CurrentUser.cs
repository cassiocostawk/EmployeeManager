using Domain.Enums;
using Domain.Interfaces;
using System.Security.Claims;

namespace Api.Services
{
    public class CurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

        public Guid UserId
        {
            get
            {
                var value = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

                return Guid.TryParse(value, out var id) ? id : Guid.Empty;
            }
        }

        public EnumEmployeeRoles Role
        {
            get
            {
                var role = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role);

                return Enum.TryParse<EnumEmployeeRoles>(role, out var parsed)
                    ? parsed
                    : EnumEmployeeRoles.Employee;
            }
        }
    }
}
