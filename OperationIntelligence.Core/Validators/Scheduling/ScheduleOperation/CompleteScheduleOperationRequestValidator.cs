using FluentValidation;
using OperationIntelligence.Core.Models.Scheduling.Requests.ScheduleOperation;
using OperationIntelligence.Core.Validators.Scheduling;

namespace OperationIntelligence.Core.Validators.Scheduling.ScheduleOperation;

public class CompleteScheduleOperationRequestValidator : AbstractValidator<CompleteScheduleOperationRequest>
{
    public CompleteScheduleOperationRequestValidator()
    {
        RuleFor(x => x.ActualEndUtc)
            .NotEmpty();

        RuleFor(x => x.CompletedQuantity)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.ScrappedQuantity)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Notes)
            .MaximumLength(SchedulingValidationConstants.NotesMaxLength);
    }
}
