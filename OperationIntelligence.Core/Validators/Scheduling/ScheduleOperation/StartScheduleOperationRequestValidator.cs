using FluentValidation;
using OperationIntelligence.Core.Models.Scheduling.Requests.ScheduleOperation;
using OperationIntelligence.Core.Validators.Scheduling;

namespace OperationIntelligence.Core.Validators.Scheduling.ScheduleOperation;

public class StartScheduleOperationRequestValidator : AbstractValidator<StartScheduleOperationRequest>
{
    public StartScheduleOperationRequestValidator()
    {
        RuleFor(x => x.ActualStartUtc)
            .NotEmpty();

        RuleFor(x => x.Notes)
            .MaximumLength(SchedulingValidationConstants.NotesMaxLength);
    }
}
