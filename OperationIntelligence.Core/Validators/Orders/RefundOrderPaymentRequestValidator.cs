using FluentValidation;

namespace OperationIntelligence.Core;

public class RefundOrderPaymentRequestValidator : AbstractValidator<RefundOrderPaymentRequest>
{
    public RefundOrderPaymentRequestValidator()
    {
        RuleFor(x => x.OrderPaymentId)
            .NotEmpty();

        RuleFor(x => x.RefundAmount)
            .GreaterThan(0);

        RuleFor(x => x.Reason)
            .MaximumLength(500);

        RuleFor(x => x.RefundedBy)
            .NotEmpty()
            .MaximumLength(150);
    }
}
