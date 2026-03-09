using FluentValidation;

namespace OperationIntelligence.Core;

public class CreateCarrierServiceRequestValidator : AbstractValidator<CreateCarrierServiceRequest>
{
    public CreateCarrierServiceRequestValidator()
    {
        RuleFor(x => x.CarrierId).NotEmpty();

        RuleFor(x => x.ServiceCode)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.EstimatedTransitDays).GreaterThanOrEqualTo(0);
    }
}
