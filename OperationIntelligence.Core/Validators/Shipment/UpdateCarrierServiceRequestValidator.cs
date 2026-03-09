using FluentValidation;

namespace OperationIntelligence.Core;

public class UpdateCarrierServiceRequestValidator : AbstractValidator<UpdateCarrierServiceRequest>
{
    public UpdateCarrierServiceRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.EstimatedTransitDays).GreaterThanOrEqualTo(0);
    }
}
