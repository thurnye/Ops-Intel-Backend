using FluentValidation;

namespace OperationIntelligence.Core;

public class UpdateDeliveryRunRequestValidator : AbstractValidator<UpdateDeliveryRunRequest>
{
    public UpdateDeliveryRunRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.DriverName).MaximumLength(150);
        RuleFor(x => x.VehicleNumber).MaximumLength(100);
        RuleFor(x => x.RouteCode).MaximumLength(100);
        RuleFor(x => x.Notes).MaximumLength(1000);

        RuleFor(x => x.PlannedEndUtc)
            .GreaterThanOrEqualTo(x => x.PlannedStartUtc);

        RuleFor(x => x.ActualEndUtc)
            .GreaterThanOrEqualTo(x => x.ActualStartUtc!.Value)
            .When(x => x.ActualStartUtc.HasValue && x.ActualEndUtc.HasValue);
    }
}
