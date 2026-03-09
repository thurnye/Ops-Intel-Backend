using FluentValidation;

namespace OperationIntelligence.Core;

public class UpdateShipmentItemRequestValidator : AbstractValidator<UpdateShipmentItemRequest>
{
    public UpdateShipmentItemRequestValidator()
    {
        RuleFor(x => x.WarehouseId).NotEmpty();
        RuleFor(x => x.UnitOfMeasureId).NotEmpty();

        RuleFor(x => x.OrderedQuantity).NonNegativeAmount();
        RuleFor(x => x.AllocatedQuantity).NonNegativeAmount();
        RuleFor(x => x.PickedQuantity).NonNegativeAmount();
        RuleFor(x => x.PackedQuantity).NonNegativeAmount();
        RuleFor(x => x.ShippedQuantity).NonNegativeAmount();
        RuleFor(x => x.DeliveredQuantity).NonNegativeAmount();
        RuleFor(x => x.ReturnedQuantity).NonNegativeAmount();
        RuleFor(x => x.UnitWeight).NonNegativeAmount();
        RuleFor(x => x.UnitVolume).NonNegativeAmount();

        RuleFor(x => x.LotNumber).MaximumLength(100);
        RuleFor(x => x.SerialNumber).MaximumLength(100);
        RuleFor(x => x.Notes).MaximumLength(1000);

        RuleFor(x => x.AllocatedQuantity)
            .LessThanOrEqualTo(x => x.OrderedQuantity);

        RuleFor(x => x.PickedQuantity)
            .LessThanOrEqualTo(x => x.AllocatedQuantity);

        RuleFor(x => x.PackedQuantity)
            .LessThanOrEqualTo(x => x.PickedQuantity);

        RuleFor(x => x.ShippedQuantity)
            .LessThanOrEqualTo(x => x.PackedQuantity);

        RuleFor(x => x.DeliveredQuantity)
            .LessThanOrEqualTo(x => x.ShippedQuantity);

        RuleFor(x => x.ReturnedQuantity)
            .LessThanOrEqualTo(x => x.DeliveredQuantity);
    }
}
