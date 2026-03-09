using FluentValidation;

namespace OperationIntelligence.Core;

public class CreateShipmentAddressRequestValidator : AbstractValidator<CreateShipmentAddressRequest>
{
    public CreateShipmentAddressRequestValidator()
    {
        RuleFor(x => x.AddressType)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.ContactName)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.CompanyName).MaximumLength(150);
        RuleFor(x => x.Phone).MaximumLength(50);
        RuleFor(x => x.Email).MaximumLength(150).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.AddressLine1)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.AddressLine2).MaximumLength(200);

        RuleFor(x => x.City)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.StateOrProvince)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.PostalCode)
            .NotEmpty()
            .MaximumLength(30);

        RuleFor(x => x.Country)
            .NotEmpty()
            .MaximumLength(100);
    }
}
