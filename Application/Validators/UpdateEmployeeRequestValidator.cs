using Application.Requests;
using Domain.Enums;
using FluentValidation;

namespace Application.Validators
{
    public class UpdateEmployeeRequestValidator : AbstractValidator<UpdateEmployeeRequest>
    {
        public UpdateEmployeeRequestValidator()
        {
            When(x => !string.IsNullOrEmpty(x.FirstName), () =>
            {
                RuleFor(x => x.FirstName)
                    .NotEqual(string.Empty).WithMessage("First Name Cannot Be Empty.")
                    .MaximumLength(60).WithMessage("First Name Must Not Exceed 60 Characters.");
            });

            When(x => !string.IsNullOrEmpty(x.LastName), () =>
            {
                RuleFor(x => x.LastName)
                    .NotEqual(string.Empty).WithMessage("Last Name Cannot Be Empty.")
                    .MaximumLength(60).WithMessage("Last Name Must Not Exceed 60 Characters.");
            });

            When(x => !string.IsNullOrEmpty(x.Password), () =>
            {
                RuleFor(x => x.Password)
                    .NotEqual(string.Empty).WithMessage("Password Cannot Be Empty.")
                    .MinimumLength(6).WithMessage("Password Must Be At Least 6 Characters.")
                    .MaximumLength(80).WithMessage("Password Must Not Exceed 60 Characters.");
            });

            When(x => !string.IsNullOrEmpty(x.BirthDate), () =>
            {
                RuleFor(x => x.BirthDate)
                    .NotEqual(string.Empty).WithMessage("Birth Date Cannot Be Empty.")
                    .Must(BeValidDate).WithMessage("Invalid Birth Date Format. Use yyyy/MM/dd.")
                    .Must(BeNotAMinor).WithMessage("Employee Must Be At Least 18 Years Old.");
            });

            When(x => x.Role.HasValue, () =>
            {
                RuleFor(x => x.Role)
                    .Must(role => Enum.IsDefined(typeof(EnumEmployeeRoles), role!.Value))
                    .WithMessage("Invalid Role. Allowed Values: 1 (Director), 2 (Leader), 3 (Employee).");
            });

            When(x => x.Phones != null && x.Phones.Any(), () =>
            {
                RuleForEach(x => x.Phones)
                    .SetValidator(new EmployeePhoneRequestValidator());
            });
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