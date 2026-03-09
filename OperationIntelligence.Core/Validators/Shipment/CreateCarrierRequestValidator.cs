using FluentValidation;

namespace OperationIntelligence.Core;

public class CreateCarrierRequestValidator : AbstractValidator<CreateCarrierRequest>
{
    public CreateCarrierRequestValidator()
    {
        RuleFor(x => x.CarrierCode)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.ContactName).MaximumLength(150);
        RuleFor(x => x.Phone).MaximumLength(50);
        RuleFor(x => x.Email).MaximumLength(150).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email));
        RuleFor(x => x.Website).MaximumLength(500);
    }
}
