using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Commands
{
    public class DeleteEmployeeCommand : IRequest
    {
        public Guid Id { get; set; }

        public DeleteEmployeeCommand(Guid id)
        {
            Id = id;
        }
    }

    public class DeleteEmployeeCommandHandler : IRequestHandler<DeleteEmployeeCommand>
    {
        private readonly IEmployeeRepository _repository;
        private readonly ICurrentUser _currentUser;

        public DeleteEmployeeCommandHandler(IEmployeeRepository repository, ICurrentUser currentUser)
        {
            _repository = repository;
            _currentUser = currentUser;
        }

        public async Task Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ValidationException("Request is null.");

            if (request.Id == Guid.Empty)
                throw new ValidationException("Invalid Employee Id.");

            var data = await _repository.GetByIdAsync(request.Id, cancellationToken);   

            if (data?.Role < _currentUser.Role)
                throw new BusinessRuleException("Unauthorized to remove Employee with higher role level.");

            await _repository.DeleteAsync(request.Id, cancellationToken);
            return;
        }
    }
}
