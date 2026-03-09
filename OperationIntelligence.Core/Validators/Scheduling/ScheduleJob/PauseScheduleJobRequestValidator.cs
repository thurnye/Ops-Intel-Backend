using FluentValidation;
using OperationIntelligence.Core.Models.Scheduling.Requests.ScheduleJob;
using OperationIntelligence.Core.Validators.Scheduling;

namespace OperationIntelligence.Core.Validators.Scheduling.ScheduleJob;

public class PauseScheduleJobRequestValidator : AbstractValidator<PauseScheduleJobRequest>
{
    public PauseScheduleJobRequestValidator()
    {
        RuleFor(x => x.Reason)
            .NotEmpty()
            .MaximumLength(SchedulingValidationConstants.NameMaxLength);

        RuleFor(x => x.Notes)
            .MaximumLength(SchedulingValidationConstants.NotesMaxLength);
    }
}
