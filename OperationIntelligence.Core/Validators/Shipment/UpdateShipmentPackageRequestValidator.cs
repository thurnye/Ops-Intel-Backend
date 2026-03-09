using FluentValidation;

namespace OperationIntelligence.Core;

public class UpdateShipmentPackageRequestValidator : AbstractValidator<UpdateShipmentPackageRequest>
{
    public UpdateShipmentPackageRequestValidator()
    {
        RuleFor(x => x.PackageType)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Length).NonNegativeAmount();
        RuleFor(x => x.Width).NonNegativeAmount();
        RuleFor(x => x.Height).NonNegativeAmount();
        RuleFor(x => x.Weight).NonNegativeAmount();
        RuleFor(x => x.DeclaredValue).NonNegativeAmount();

        RuleFor(x => x.TrackingNumber).MaximumLength(100);
        RuleFor(x => x.Barcode).MaximumLength(100);
        RuleFor(x => x.LabelUrl).MaximumLength(1000);
    }
}
