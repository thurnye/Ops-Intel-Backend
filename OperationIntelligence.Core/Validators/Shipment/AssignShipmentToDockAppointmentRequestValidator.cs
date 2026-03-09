using FluentValidation;

namespace OperationIntelligence.Core;

public class AssignShipmentToDockAppointmentRequestValidator : AbstractValidator<AssignShipmentToDockAppointmentRequest>
{
    public AssignShipmentToDockAppointmentRequestValidator()
    {
        RuleFor(x => x.DockAppointmentId).NotEmpty();
    }
}
