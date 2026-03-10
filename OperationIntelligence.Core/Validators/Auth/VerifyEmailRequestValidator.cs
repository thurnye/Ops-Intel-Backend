using FluentValidation;

namespace OperationIntelligence.Core
{
    public class VerifyEmailRequestValidator : AbstractValidator<VerifyEmailRequest>
    {
        public VerifyEmailRequestValidator()
        {
            RuleFor(x => x.Token)
                .NotEmpty();
        }
    }
}
