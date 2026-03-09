using FluentValidation;

namespace OperationIntelligence.Core;

public class AssignShipmentToDeliveryRunRequestValidator : AbstractValidator<AssignShipmentToDeliveryRunRequest>
{
    public AssignShipmentToDeliveryRunRequestValidator()
    {
        RuleFor(x => x.DeliveryRunId).NotEmpty();
    }
}
