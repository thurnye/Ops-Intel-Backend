using FluentValidation;

namespace OperationIntelligence.Core;

public class CreateDockAppointmentRequestValidator : AbstractValidator<CreateDockAppointmentRequest>
{
    public CreateDockAppointmentRequestValidator()
    {
        RuleFor(x => x.AppointmentNumber)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.WarehouseId).NotEmpty();

        RuleFor(x => x.DockCode).MaximumLength(50);
        RuleFor(x => x.TrailerNumber).MaximumLength(100);
        RuleFor(x => x.DriverName).MaximumLength(150);
        RuleFor(x => x.Notes).MaximumLength(1000);

        RuleFor(x => x.ScheduledEndUtc)
            .GreaterThanOrEqualTo(x => x.ScheduledStartUtc);
    }
}
