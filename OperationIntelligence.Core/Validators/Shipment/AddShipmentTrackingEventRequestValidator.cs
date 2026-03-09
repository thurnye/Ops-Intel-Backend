using FluentValidation;

namespace OperationIntelligence.Core;

public class AddShipmentTrackingEventRequestValidator : AbstractValidator<AddShipmentTrackingEventRequest>
{
    public AddShipmentTrackingEventRequestValidator()
    {
        RuleFor(x => x.EventCode)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.EventName)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Description).MaximumLength(1000);
        RuleFor(x => x.LocationName).MaximumLength(150);
        RuleFor(x => x.City).MaximumLength(100);
        RuleFor(x => x.StateOrProvince).MaximumLength(100);
        RuleFor(x => x.Country).MaximumLength(100);
        RuleFor(x => x.CarrierStatusCode).MaximumLength(50);

        RuleFor(x => x.Source)
            .NotEmpty()
            .MaximumLength(50);
    }
}
