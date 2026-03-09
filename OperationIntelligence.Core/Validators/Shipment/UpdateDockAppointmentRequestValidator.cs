using FluentValidation;

namespace OperationIntelligence.Core;

public class UpdateDockAppointmentRequestValidator : AbstractValidator<UpdateDockAppointmentRequest>
{
    public UpdateDockAppointmentRequestValidator()
    {
        RuleFor(x => x.DockCode).MaximumLength(50);
        RuleFor(x => x.TrailerNumber).MaximumLength(100);
        RuleFor(x => x.DriverName).MaximumLength(150);
        RuleFor(x => x.Notes).MaximumLength(1000);

        RuleFor(x => x.ScheduledEndUtc)
            .GreaterThanOrEqualTo(x => x.ScheduledStartUtc);

        RuleFor(x => x.ActualDepartureUtc)
            .GreaterThanOrEqualTo(x => x.ActualArrivalUtc!.Value)
            .When(x => x.ActualArrivalUtc.HasValue && x.ActualDepartureUtc.HasValue);
    }
}
