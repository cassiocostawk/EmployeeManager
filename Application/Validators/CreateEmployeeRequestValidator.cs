using Application.Requests;
using Domain.Enums;
using FluentValidation;

namespace Application.Validators
{
    public class CreateEmployeeRequestValidator : AbstractValidator<CreateEmployeeRequest>
    {
        public CreateEmployeeRequestValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First Name Is Required.")
                .NotEqual(string.Empty).WithMessage("First Name Cannot Be Empty.")
                .MaximumLength(60).WithMessage("First Name Must Not Exceed 60 Characters.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last Name Is Required.")
                .NotEqual(string.Empty).WithMessage("Last Name Cannot Be Empty.")
                .MaximumLength(60).WithMessage("Last Name Must Not Exceed 60 Characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email Is Required.")
                .NotEqual(string.Empty).WithMessage("Email Cannot Be Empty.")
                .EmailAddress().WithMessage("Invalid Email Format.")
                .MaximumLength(100).WithMessage("Email Must Not Exceed 100 Characters.");

            RuleFor(x => x.DocNumber)
                .NotEmpty().WithMessage("Document Number Is Required.")
                .NotEqual(string.Empty).WithMessage("Document Number Cannot Be Empty.")
                .MaximumLength(20).WithMessage("Document Number Must Not Exceed 20 Characters.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password Is Required.")
                .NotEqual(string.Empty).WithMessage("Password Cannot Be Empty.")
                .MinimumLength(6).WithMessage("Password Must Be At Least 6 Characters.")
                .MaximumLength(80).WithMessage("Password Must Not Exceed 255 Characters.");

            RuleFor(x => x.BirthDate)
                .NotEmpty().WithMessage("Birth Date Is Required.")
                .NotEqual(string.Empty).WithMessage("Birth Date Cannot Be Empty.")
                .Must(BeValidDate).WithMessage("Invalid Birth Date Format. Use yyyy/MM/dd.")
                .Must(BeNotAMinor).WithMessage("Employee Must Be At Least 18 Years Old.");

            RuleFor(x => x.Role)
                .IsInEnum().WithMessage("Invalid Role. Allowed Values: 1 (Director), 2 (Leader), 3 (Employee).");

            RuleForEach(x => x.Phones)
                .SetValidator(new EmployeePhoneRequestValidator());
        }

        private static bool BeValidDate(string? date)
        {
            if (string.IsNullOrEmpty(date))
                return false;

            return DateOnly.TryParseExact(date, "yyyy/MM/dd", out _);
        }

        private static bool BeNotAMinor(string? date)
        {
            if (string.IsNullOrEmpty(date))
                return false;

            if (DateOnly.TryParseExact(date, "yyyy/MM/dd", out var birthDate))
            {
                var today = DateOnly.FromDateTime(DateTime.Now);
                var age = today.Year - birthDate.Year;

                if (birthDate > today.AddYears(-age))
                    age--;

                return age >= 18;
            }

            return false;
        }
    }
}