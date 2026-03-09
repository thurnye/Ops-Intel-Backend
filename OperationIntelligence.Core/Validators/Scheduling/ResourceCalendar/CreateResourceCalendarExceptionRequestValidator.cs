using FluentValidation;
using OperationIntelligence.Core.Models.Scheduling.Requests.ResourceCalendar;
using OperationIntelligence.Core.Validators.Scheduling;

namespace OperationIntelligence.Core.Validators.Scheduling.ResourceCalendar;

public class CreateResourceCalendarExceptionRequestValidator : AbstractValidator<CreateResourceCalendarExceptionRequest>
{
    public CreateResourceCalendarExceptionRequestValidator()
    {
        RuleFor(x => x.ResourceCalendarId).NotEmpty();

        RuleFor(x => x.ExceptionEndUtc)
            .GreaterThanOrEqualTo(x => x.ExceptionStartUtc);

        RuleFor(x => x.ExceptionType)
            .InclusiveBetween(1, 7);

        RuleFor(x => x.Reason)
            .NotEmpty()
            .MaximumLength(SchedulingValidationConstants.NameMaxLength);

        RuleFor(x => x.Notes)
            .MaximumLength(SchedulingValidationConstants.NotesMaxLength);
    }
}
