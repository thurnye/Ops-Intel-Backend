using FluentValidation;

namespace OperationIntelligence.Core;

public class OrderQueryRequestValidator : AbstractValidator<OrderQueryRequest>
{
    public OrderQueryRequestValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(200);

        RuleFor(x => x.SearchTerm)
            .MaximumLength(200);

        RuleFor(x => x.Status)
            .IsInEnum()
            .When(x => x.Status.HasValue);

        RuleFor(x => x.OrderType)
            .IsInEnum()
            .When(x => x.OrderType.HasValue);
    }
}
