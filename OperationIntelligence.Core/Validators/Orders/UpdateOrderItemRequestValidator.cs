using FluentValidation;

namespace OperationIntelligence.Core;

public class UpdateOrderItemRequestValidator : AbstractValidator<UpdateOrderItemRequest>
{
    public UpdateOrderItemRequestValidator()
    {
        RuleFor(x => x.QuantityOrdered)
            .GreaterThan(0);

        RuleFor(x => x.UnitPrice)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.DiscountAmount)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.TaxAmount)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Remarks)
            .MaximumLength(500);

        RuleFor(x => x.SortOrder)
            .GreaterThanOrEqualTo(0);
    }
}
