using FluentValidation;
using OperationIntelligence.Core.Models.Scheduling.Requests.ResourceCalendar;
using OperationIntelligence.Core.Validators.Scheduling;

namespace OperationIntelligence.Core.Validators.Scheduling.ResourceCalendar;

public class UpdateResourceCalendarRequestValidator : AbstractValidator<UpdateResourceCalendarRequest>
{
    public UpdateResourceCalendarRequestValidator()
    {
        RuleFor(x => x.CalendarName)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.TimeZone)
            .NotEmpty()
            .MaximumLength(SchedulingValidationConstants.TimeZoneMaxLength);
    }
}
