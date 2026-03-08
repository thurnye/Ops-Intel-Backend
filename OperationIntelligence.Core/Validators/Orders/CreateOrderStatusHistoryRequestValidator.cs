using FluentValidation;

namespace OperationIntelligence.Core;

public class CreateOrderStatusHistoryRequestValidator : AbstractValidator<CreateOrderStatusHistoryRequest>
{
    public CreateOrderStatusHistoryRequestValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty();

        RuleFor(x => x.FromStatus)
            .IsInEnum();

        RuleFor(x => x.ToStatus)
            .IsInEnum();

        RuleFor(x => x.ChangedBy)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Reason)
            .MaximumLength(500);

        RuleFor(x => x.Comments)
            .MaximumLength(1000);
    }
}
