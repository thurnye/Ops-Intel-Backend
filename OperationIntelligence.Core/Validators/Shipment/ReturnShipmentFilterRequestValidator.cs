using FluentValidation;

namespace OperationIntelligence.Core;

public class ReturnShipmentFilterRequestValidator : AbstractValidator<ReturnShipmentFilterRequest>
{
    public ReturnShipmentFilterRequestValidator()
    {
        RuleFor(x => x.PageNumber).ValidPageNumber();
        RuleFor(x => x.PageSize).ValidPageSize();

        RuleFor(x => x.RequestedToUtc)
            .GreaterThanOrEqualTo(x => x.RequestedFromUtc!.Value)
            .When(x => x.RequestedFromUtc.HasValue && x.RequestedToUtc.HasValue);
    }
}
