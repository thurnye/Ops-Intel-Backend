using FluentValidation;

namespace OperationIntelligence.Core;

public class AddShipmentPackageItemRequestValidator : AbstractValidator<AddShipmentPackageItemRequest>
{
    public AddShipmentPackageItemRequestValidator()
    {
        RuleFor(x => x.ShipmentItemId).NotEmpty();
        RuleFor(x => x.Quantity).PositiveAmount();
    }
}
