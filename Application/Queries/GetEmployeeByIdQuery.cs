using Application.Responses;
using AutoMapper;
using Domain.Interfaces;
using MediatR;

namespace Application.Queries
{
    public class GetEmployeeByIdQuery :IRequest<EmployeeResponse>
    {
        public Guid Id { get; set; }

        public GetEmployeeByIdQuery(Guid id)
        {
            Id = id;
        }
    }

    public class GetEmployeeByIdQueryHandler : IRequestHandler<GetEmployeeByIdQuery, EmployeeResponse>
    {
        private readonly IMapper _mapper;
        private readonly IEmployeeRepository _repository;

        public GetEmployeeByIdQueryHandler(IMapper mapper, IEmployeeRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<EmployeeResponse> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentException("Request is null."); // TODO: Exception Middleware stage - implement on Middleware

            if (request.Id == Guid.Empty)
                throw new ArgumentException("Invalid Employee Id."); // TODO: Exception Middleware stage - implement on Middleware

            var data = await _repository.GetByIdAsync(request.Id, cancellationToken);

            if (data == null)
                throw new KeyNotFoundException("Employee not found."); // TODO: Exception Middleware stage - implement on Middleware

            var mappedData = _mapper.Map<EmployeeResponse>(data);

            return mappedData;
        }
    }
}
