using FluentValidation;

namespace OperationIntelligence.Core;

public class CreateReturnShipmentRequestValidator : AbstractValidator<CreateReturnShipmentRequest>
{
    public CreateReturnShipmentRequestValidator()
    {
        RuleFor(x => x.ReturnShipmentNumber)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.ShipmentId).NotEmpty();

        RuleFor(x => x.OriginAddressId).NotEmpty();
        RuleFor(x => x.DestinationAddressId).NotEmpty();

        RuleFor(x => x.TrackingNumber).MaximumLength(100);

        RuleFor(x => x.ReasonCode)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.ReasonDescription).MaximumLength(1000);
        RuleFor(x => x.Notes).MaximumLength(1000);

        RuleFor(x => x.CarrierServiceId)
            .Must((request, carrierServiceId) => !carrierServiceId.HasValue || request.CarrierId.HasValue)
            .WithMessage("CarrierId is required when CarrierServiceId is provided.");
    }
}
