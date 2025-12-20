using Application.Requests;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Commands
{
    public class CreateEmployeeCommand : IRequest
    {
        public CreateEmployeeRequest Request { get; set; }

        public CreateEmployeeCommand(CreateEmployeeRequest request)
        {
            Request = request;
        }
    }

    public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand>
    {
        private readonly IMapper _mapper;
        private readonly IEmployeeRepository _repository;

        public CreateEmployeeCommandHandler(IMapper mapper, IEmployeeRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public async Task Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
        {
            if (request.Request == null)
                throw new ValidationException("Request body is null.");

            // TODO: Auth stage - Uncomment and implement _currentUser
            /*if (_currentUser.Role < request.Request.Role)
                throw new BusinessRuleException("Unauthorized to create Employee with higher role level.");*/

            var existingEmailEmployee = await _repository.GetByEmailAsync(request.Request.Email, cancellationToken);

            if (existingEmailEmployee != null)
                throw new BusinessRuleException("Email already exists");

            var existingDocNumberEmployee = await _repository.GetByDocNumberAsync(request.Request.DocNumber, cancellationToken);

            if (existingDocNumberEmployee != null)
                throw new BusinessRuleException("Document Number already exists.");

            var entity = _mapper.Map<Employee>(request.Request);

            entity.Phones = request.Request.Phones?
                .Select(phoneRequest => _mapper.Map<EmployeePhone>(phoneRequest))
                .ToList();
                
            // TODO: PasswordHasher stage - Hash Password

            await _repository.CreateAsync(entity, cancellationToken);

            return;
        }
    }
}
