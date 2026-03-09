using FluentValidation;

namespace OperationIntelligence.Core;

public class AddShipmentChargeRequestValidator : AbstractValidator<AddShipmentChargeRequest>
{
    public AddShipmentChargeRequestValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Amount).NonNegativeAmount();
        RuleFor(x => x.CurrencyCode).ValidCurrencyCode();
    }
}
