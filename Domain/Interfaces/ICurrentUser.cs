using Domain.Enums;

namespace Domain.Interfaces
{
    public interface ICurrentUser
    {
        Guid UserId { get;  }
        EnumEmployeeRoles Role { get; }
        bool IsAuthenticated { get; }
    }
}
