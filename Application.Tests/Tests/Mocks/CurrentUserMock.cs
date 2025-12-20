using Domain.Enums;
using Domain.Interfaces;

namespace Application.Tests.Tests.Mocks
{
    public class CurrentUserMock : ICurrentUser
    {
        public Guid UserId { get; set; } = Guid.NewGuid();
        public EnumEmployeeRoles Role { get; set; } = EnumEmployeeRoles.Employee;
        public bool IsAuthenticated { get; set; } = true;

    }
}
