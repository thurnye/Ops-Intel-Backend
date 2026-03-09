using FluentValidation;
using OperationIntelligence.Core.Models.Scheduling.Requests.ScheduleJob;
using OperationIntelligence.Core.Validators.Scheduling;

namespace OperationIntelligence.Core.Validators.Scheduling.ScheduleJob;

public class ReleaseScheduleJobRequestValidator : AbstractValidator<ReleaseScheduleJobRequest>
{
    public ReleaseScheduleJobRequestValidator()
    {
        RuleFor(x => x.Reason)
            .MaximumLength(SchedulingValidationConstants.NotesMaxLength);

        RuleFor(x => x.Notes)
            .MaximumLength(SchedulingValidationConstants.NotesMaxLength);
    }
}
