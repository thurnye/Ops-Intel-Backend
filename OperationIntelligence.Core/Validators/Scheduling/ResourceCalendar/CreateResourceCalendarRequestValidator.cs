using FluentValidation;
using OperationIntelligence.Core.Models.Scheduling.Requests.ResourceCalendar;
using OperationIntelligence.Core.Validators.Scheduling;

namespace OperationIntelligence.Core.Validators.Scheduling.ResourceCalendar;

public class CreateResourceCalendarRequestValidator : AbstractValidator<CreateResourceCalendarRequest>
{
    public CreateResourceCalendarRequestValidator()
    {
        RuleFor(x => x.ResourceId).NotEmpty();

        RuleFor(x => x.ResourceType)
            .InclusiveBetween(1, 6);

        RuleFor(x => x.CalendarName)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.TimeZone)
            .NotEmpty()
            .MaximumLength(SchedulingValidationConstants.TimeZoneMaxLength);
    }
}
