using FluentValidation;

namespace OperationIntelligence.Core;

public class UpdateShipmentRequestValidator : AbstractValidator<UpdateShipmentRequest>
{
    public UpdateShipmentRequestValidator()
    {
        RuleFor(x => x.OriginAddressId).NotEmpty();
        RuleFor(x => x.DestinationAddressId).NotEmpty();

        RuleFor(x => x.CustomerReference).MaximumLength(100);
        RuleFor(x => x.ExternalReference).MaximumLength(100);
        RuleFor(x => x.TrackingNumber).MaximumLength(100);
        RuleFor(x => x.MasterTrackingNumber).MaximumLength(100);
        RuleFor(x => x.ShippingTerms).MaximumLength(100);
        RuleFor(x => x.Incoterm).MaximumLength(50);
        RuleFor(x => x.Notes).MaximumLength(1000);
        RuleFor(x => x.InternalNotes).MaximumLength(1000);

        RuleFor(x => x.CurrencyCode).ValidCurrencyCode();

        RuleFor(x => x.PlannedDeliveryDateUtc)
            .GreaterThanOrEqualTo(x => x.PlannedShipDateUtc!.Value)
            .When(x => x.PlannedShipDateUtc.HasValue && x.PlannedDeliveryDateUtc.HasValue);

        RuleFor(x => x.ScheduledPickupEndUtc)
            .GreaterThanOrEqualTo(x => x.ScheduledPickupStartUtc!.Value)
            .When(x => x.ScheduledPickupStartUtc.HasValue && x.ScheduledPickupEndUtc.HasValue);

        RuleFor(x => x.CarrierServiceId)
            .Must((request, carrierServiceId) => !carrierServiceId.HasValue || request.CarrierId.HasValue)
            .WithMessage("CarrierId is required when CarrierServiceId is provided.");
    }
}
