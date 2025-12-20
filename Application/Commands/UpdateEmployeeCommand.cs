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
        private readonly IPasswordHasher _passwordHasher;
        private readonly ICurrentUser _currentUser;

        public UpdateEmployeeCommandHandler(IMapper mapper, IEmployeeRepository repository, IPasswordHasher passwordHasher, ICurrentUser currentUser)
        {
            _mapper = mapper;
            _repository = repository;
            _passwordHasher = passwordHasher;
            _currentUser = currentUser;
        }
        public async Task Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
        {
            if (request.Request == null)
                throw new ValidationException("Request body is null.");

            if (request.Id == Guid.Empty)
                throw new ValidationException("Invalid Employee Id.");

            if (request.Request.Role < _currentUser.Role)
                throw new BusinessRuleException("Unauthorized to update Employee with higher role level.");

            var oldData = await _repository.GetByIdAsync(request.Id, cancellationToken);

            if (oldData == null)
                throw new NotFoundException("Employee not found.");

            var updatedData = _mapper.Map<UpdateEmployeeRequest, Employee>(request.Request, oldData);

            if (!string.IsNullOrWhiteSpace(request.Request.Password))
                updatedData.Password = _passwordHasher.Hash(request.Request.Password);

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
