using Application.Responses;
using AutoMapper;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Queries
{
    public class LoginQuery : IRequest<LoginResponse>
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public LoginQuery(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }

    public class LoginQueryHandler : IRequestHandler<LoginQuery, LoginResponse>
    {
        private readonly IMapper _mapper;
        private readonly IEmployeeRepository _repository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenService _jwtTokenService;

        public LoginQueryHandler(IMapper mapper, IEmployeeRepository repository, IPasswordHasher passwordHasher, IJwtTokenService jwtTokenService)
        {
            _mapper = mapper;
            _repository = repository;
            _passwordHasher = passwordHasher;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<LoginResponse> Handle(LoginQuery request, CancellationToken cancellationToken)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                throw new ValidationException("Email and Password must be provided.");

            var data = await _repository.GetByEmailAsync(request.Email, cancellationToken);

            if (data == null || !_passwordHasher.Verify(request.Password, data.Password))
                throw new ValidationException("Invalid email or password.");

            var token = _jwtTokenService.Generate(data);

            return new LoginResponse
            {
                AccessToken = token.AccessToken,
                CurrentUser = _mapper.Map<EmployeeBasicResponse>(data),
                ExpiresAt = token.ExpiresAt,
            };
        }
    }
}
