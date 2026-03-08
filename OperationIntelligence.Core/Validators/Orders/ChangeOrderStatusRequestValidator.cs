using FluentValidation;

namespace OperationIntelligence.Core;

public class ChangeOrderStatusRequestValidator : AbstractValidator<ChangeOrderStatusRequest>
{
    public ChangeOrderStatusRequestValidator()
    {
        RuleFor(x => x.NewStatus)
            .IsInEnum();

        RuleFor(x => x.ChangedBy)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Reason)
            .MaximumLength(500);

        RuleFor(x => x.Comments)
            .MaximumLength(1000);

        When(x => x.NewStatus == OrderStatus.Cancelled, () =>
        {
            RuleFor(x => x.Reason)
                .NotEmpty()
                .WithMessage(OrderValidationMessages.CancellationReasonRequired);
        });
    }
}
