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

        public DeleteEmployeeCommandHandler(IEmployeeRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ValidationException("Request is null.");

            if (request.Id == Guid.Empty)
                throw new ValidationException("Invalid Employee Id.");

            await _repository.DeleteAsync(request.Id, cancellationToken);
            return;
        }
    }
}
