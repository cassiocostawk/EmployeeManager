using Application.Requests;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Commands
{
    public class UpdateEmployeeCommand : IRequest
    {
        public Guid Id { get; set; }
        public UpdateEmployeeRequest Request { get; set; }

        public UpdateEmployeeCommand(Guid id, UpdateEmployeeRequest request)
        {
            Id = id;
            Request = request;
        }
    }

    public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand>
    {
        private readonly IMapper _mapper;
        private readonly IEmployeeRepository _repository;

        public UpdateEmployeeCommandHandler(IMapper mapper, IEmployeeRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }
        public async Task Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
        {
            if (request.Request == null)
                throw new ValidationException("Request body is null.");

            if (request.Id == Guid.Empty)
                throw new ValidationException("Invalid Employee Id.");

            // TODO: Auth and Exception Middleware stages - Business Logic Validation

            var oldData = await _repository.GetByIdAsync(request.Id, cancellationToken);

            if (oldData == null)
                throw new NotFoundException("Employee not found.");

            var updatedData = _mapper.Map<UpdateEmployeeRequest, Employee>(request.Request, oldData);

            // TODO: PasswordHasher stage - Hash Password if not empty

            var updatedPhonesData = GeneratePhonesFromRequest(request.Request.Phones, oldData.Phones);
            updatedData.Phones = updatedPhonesData;

            await _repository.UpdateAsync(updatedData, cancellationToken);

            return;
        }

        private static List<EmployeePhone> GeneratePhonesFromRequest(IEnumerable<EmployeePhoneRequest>? requestPhones, IEnumerable<EmployeePhone>? oldPhones)
        {
            var updatedPhones = new List<EmployeePhone>();

            if (oldPhones != null)
            {
                foreach (var oldPhone in oldPhones)
                {
                    var requestPhone = requestPhones?.FirstOrDefault(phoneRequest => phoneRequest.Id == oldPhone.Id);

                    if (requestPhone != null)
                    {
                        oldPhone.PhoneNumber = requestPhone.PhoneNumber;
                        updatedPhones.Add(oldPhone);
                    }
                }
            }

            var phonesToAdd = requestPhones?.Where(phoneRequest => phoneRequest.Id == null);

            if (phonesToAdd != null)
            {
                foreach (var newPhone in phonesToAdd)
                {
                    updatedPhones.Add(new EmployeePhone { PhoneNumber = newPhone.PhoneNumber });
                }
            }

            return updatedPhones;
        }

    }
}
