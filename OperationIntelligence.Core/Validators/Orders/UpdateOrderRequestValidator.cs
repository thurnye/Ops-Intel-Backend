using FluentValidation;

namespace OperationIntelligence.Core;

public class UpdateOrderRequestValidator : AbstractValidator<UpdateOrderRequest>
{
    public UpdateOrderRequestValidator()
    {
        RuleFor(x => x.CustomerName)
            .MaximumLength(200);

        RuleFor(x => x.CustomerEmail)
            .MaximumLength(150)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.CustomerEmail));

        RuleFor(x => x.CustomerPhone)
            .MaximumLength(50);

        RuleFor(x => x.Priority)
            .IsInEnum();

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
    }
}
