using Domain.Interfaces;

namespace Application.Tests.Tests.Mocks
{
    public class PasswordHasherMock : IPasswordHasher
    {
        public string Hash(string password) => $"hashed_{password}";

        public bool Verify(string password, string hash)
        {
            return hash == $"hashed_{password}";
        }
    }
}
