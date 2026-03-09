using FluentValidation;

namespace OperationIntelligence.Core;

public class UpdateShipmentStatusRequestValidator : AbstractValidator<UpdateShipmentStatusRequest>
{
    public UpdateShipmentStatusRequestValidator()
    {
        RuleFor(x => x.Reason).MaximumLength(500);
    }
}
