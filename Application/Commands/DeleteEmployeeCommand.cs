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
                throw new ArgumentException("Request is null."); // TODO: Exception Middleware stage - implement on Middleware

            if (request.Id == Guid.Empty)
                throw new ArgumentException("Invalid Employee Id."); // TODO: Exception Middleware stage - implement on Middleware

            await _repository.DeleteAsync(request.Id, cancellationToken);
            return;
        }
    }
}
