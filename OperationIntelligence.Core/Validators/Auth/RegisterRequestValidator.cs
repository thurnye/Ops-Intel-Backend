using FluentValidation;

namespace OperationIntelligence.Core
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(256);

            RuleFor(x => x.UserName)
                .MaximumLength(100)
                .Matches("^[a-zA-Z0-9._-]+$")
                .When(x => !string.IsNullOrWhiteSpace(x.UserName))
                .WithMessage("Username can only contain letters, numbers, dots, underscores, and hyphens.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(8)
                .MaximumLength(128)
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches("[0-9]").WithMessage("Password must contain at least one number.")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password)
                .WithMessage("Confirm password must match password.");

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.LastName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.PhoneNumber)
                .MaximumLength(50);

            RuleFor(x => x.AddressLine1)
                .MaximumLength(200);

            RuleFor(x => x.AddressLine2)
                .MaximumLength(200);

            RuleFor(x => x.City)
                .MaximumLength(100);

            RuleFor(x => x.StateOrProvince)
                .MaximumLength(100);

            RuleFor(x => x.Country)
                .MaximumLength(100);

            RuleFor(x => x.PostalCode)
                .MaximumLength(20);
        }
    }
}
