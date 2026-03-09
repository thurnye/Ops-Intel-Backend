using FluentValidation;

namespace OperationIntelligence.Core;

public class AddShipmentInsuranceRequestValidator : AbstractValidator<AddShipmentInsuranceRequest>
{
    public AddShipmentInsuranceRequestValidator()
    {
        RuleFor(x => x.ProviderName)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.PolicyNumber).MaximumLength(100);
        RuleFor(x => x.InsuredAmount).NonNegativeAmount();
        RuleFor(x => x.PremiumAmount).NonNegativeAmount();
        RuleFor(x => x.CurrencyCode).ValidCurrencyCode();
        RuleFor(x => x.Notes).MaximumLength(1000);

        RuleFor(x => x.ExpiryDateUtc)
            .GreaterThanOrEqualTo(x => x.EffectiveDateUtc)
            .When(x => x.ExpiryDateUtc.HasValue);
    }
}
