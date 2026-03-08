using FluentValidation;

namespace OperationIntelligence.Core;

public class CreateOrderAddressRequestValidator : AbstractValidator<CreateOrderAddressRequest>
{
    public CreateOrderAddressRequestValidator(bool skipOrderIdValidation = false)
    {
        if (!skipOrderIdValidation)
        {
            RuleFor(x => x.OrderId)
                .NotEmpty();
        }

        RuleFor(x => x.AddressType)
            .IsInEnum();

        RuleFor(x => x.ContactName)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.CompanyName)
            .MaximumLength(150);

        RuleFor(x => x.AddressLine1)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.AddressLine2)
            .MaximumLength(200);

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

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(50);

        RuleFor(x => x.Email)
            .MaximumLength(150)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.Email));
    }
}
