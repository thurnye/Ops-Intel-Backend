using FluentValidation;

namespace OperationIntelligence.Core
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.EmailOrUserName)
                .NotEmpty()
                .MaximumLength(256);

            RuleFor(x => x.Password)
                .NotEmpty()
                .MaximumLength(128);
        }
    }
}
