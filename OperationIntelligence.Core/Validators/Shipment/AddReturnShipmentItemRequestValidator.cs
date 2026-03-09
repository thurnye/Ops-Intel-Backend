using FluentValidation;

namespace OperationIntelligence.Core;

public class AddReturnShipmentItemRequestValidator : AbstractValidator<AddReturnShipmentItemRequest>
{
    public AddReturnShipmentItemRequestValidator()
    {
        RuleFor(x => x.ShipmentItemId).NotEmpty();
        RuleFor(x => x.ReturnedQuantity).PositiveAmount();
        RuleFor(x => x.ReturnCondition).MaximumLength(100);
        RuleFor(x => x.InspectionResult).MaximumLength(200);
        RuleFor(x => x.Notes).MaximumLength(1000);
    }
}
