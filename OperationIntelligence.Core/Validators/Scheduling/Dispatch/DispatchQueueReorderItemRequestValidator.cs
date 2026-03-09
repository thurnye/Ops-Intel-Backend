using FluentValidation;
using OperationIntelligence.Core.Models.Scheduling.Requests.Dispatch;

namespace OperationIntelligence.Core.Validators.Scheduling.Dispatch;

public class DispatchQueueReorderItemRequestValidator : AbstractValidator<DispatchQueueReorderItemRequest>
{
    public DispatchQueueReorderItemRequestValidator()
    {
        RuleFor(x => x.DispatchQueueItemId).NotEmpty();

        RuleFor(x => x.QueuePosition)
            .GreaterThan(0);
    }
}
