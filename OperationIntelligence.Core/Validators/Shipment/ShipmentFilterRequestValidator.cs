using FluentValidation;

namespace OperationIntelligence.Core;

public class ShipmentFilterRequestValidator : AbstractValidator<ShipmentFilterRequest>
{
    public ShipmentFilterRequestValidator()
    {
        RuleFor(x => x.PageNumber).ValidPageNumber();
        RuleFor(x => x.PageSize).ValidPageSize();

        RuleFor(x => x.PlannedShipDateToUtc)
            .GreaterThanOrEqualTo(x => x.PlannedShipDateFromUtc!.Value)
            .When(x => x.PlannedShipDateFromUtc.HasValue && x.PlannedShipDateToUtc.HasValue);
    }
}
