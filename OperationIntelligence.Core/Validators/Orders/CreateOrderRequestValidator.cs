using FluentValidation;

namespace OperationIntelligence.Core;

public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(x => x.OrderType)
            .IsInEnum();

        RuleFor(x => x.Priority)
            .IsInEnum();

        RuleFor(x => x.Channel)
            .IsInEnum();

        RuleFor(x => x.CurrencyCode)
            .NotEmpty()
            .MaximumLength(10);

        RuleFor(x => x.CustomerName)
            .MaximumLength(200);

        RuleFor(x => x.CustomerEmail)
            .MaximumLength(150)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.CustomerEmail));

        RuleFor(x => x.CustomerPhone)
            .MaximumLength(50);

        RuleFor(x => x.ReferenceNumber)
            .MaximumLength(100);

        RuleFor(x => x.CustomerPurchaseOrderNumber)
            .MaximumLength(100);

        RuleFor(x => x.Notes)
            .MaximumLength(2000);

        RuleFor(x => x.RequiredDateUtc)
            .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
            .When(x => x.RequiredDateUtc.HasValue)
            .WithMessage(OrderValidationMessages.RequiredDateCannotBePast);

        RuleFor(x => x.Items)
            .NotNull()
            .Must(x => x.Count > 0)
            .WithMessage(OrderValidationMessages.AtLeastOneOrderItemRequired);

        RuleForEach(x => x.Items)
            .SetValidator(new CreateOrderItemRequestValidator(skipOrderIdValidation: true));

        RuleForEach(x => x.Addresses)
            .SetValidator(new CreateOrderAddressRequestValidator(skipOrderIdValidation: true));
    }
}
