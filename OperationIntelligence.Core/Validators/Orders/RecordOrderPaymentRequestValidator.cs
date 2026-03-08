using FluentValidation;

namespace OperationIntelligence.Core;

public class RecordOrderPaymentRequestValidator : AbstractValidator<RecordOrderPaymentRequest>
{
    public RecordOrderPaymentRequestValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty();

        RuleFor(x => x.PaymentMethod)
            .IsInEnum();

        RuleFor(x => x.PaymentProvider)
            .IsInEnum();

        RuleFor(x => x.Amount)
            .GreaterThan(0);

        RuleFor(x => x.FeeAmount)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.CurrencyCode)
            .NotEmpty()
            .MaximumLength(10);

        RuleFor(x => x.ExternalTransactionId)
            .MaximumLength(150);

        RuleFor(x => x.ExternalPaymentIntentId)
            .MaximumLength(150);

        RuleFor(x => x.PayerName)
            .MaximumLength(200);

        RuleFor(x => x.PayerEmail)
            .MaximumLength(150)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.PayerEmail));

        RuleFor(x => x.Last4)
            .MaximumLength(10);

        RuleFor(x => x.AuthorizationCode)
            .MaximumLength(100);

        RuleFor(x => x.ReceiptNumber)
            .MaximumLength(100);

        RuleFor(x => x.Notes)
            .MaximumLength(1000);

        RuleFor(x => x.RecordedBy)
            .NotEmpty()
            .MaximumLength(150);
    }
}
