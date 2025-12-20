using Application.Requests;
using FluentValidation;

namespace Application.Validators
{
    public class EmployeePhoneRequestValidator : AbstractValidator<EmployeePhoneRequest>
    {
        public EmployeePhoneRequestValidator()
        {
            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone Number Is Required.")
                .NotEqual(string.Empty).WithMessage("Phone Number Cannot Be Empty.")
                .MaximumLength(25).WithMessage("Phone Number Must Not Exceed 25 Characters.");
        }
    }
}