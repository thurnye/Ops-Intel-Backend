using FluentValidation;

namespace OperationIntelligence.Core;

public class CreateDeliveryRunRequestValidator : AbstractValidator<CreateDeliveryRunRequest>
{
    public CreateDeliveryRunRequestValidator()
    {
        RuleFor(x => x.RunNumber)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.WarehouseId).NotEmpty();

        RuleFor(x => x.DriverName).MaximumLength(150);
        RuleFor(x => x.VehicleNumber).MaximumLength(100);
        RuleFor(x => x.RouteCode).MaximumLength(100);
        RuleFor(x => x.Notes).MaximumLength(1000);

        RuleFor(x => x.PlannedEndUtc)
            .GreaterThanOrEqualTo(x => x.PlannedStartUtc);
    }
}
