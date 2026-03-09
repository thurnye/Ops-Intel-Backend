using FluentValidation;
using OperationIntelligence.Core.Models.Scheduling.Requests.Capacity;
using OperationIntelligence.Core.Validators.Scheduling;

namespace OperationIntelligence.Core.Validators.Scheduling.Capacity;

public class UpdateCapacityReservationRequestValidator : AbstractValidator<UpdateCapacityReservationRequest>
{
    public UpdateCapacityReservationRequestValidator()
    {
        RuleFor(x => x.ReservedEndUtc)
            .GreaterThanOrEqualTo(x => x.ReservedStartUtc);

        RuleFor(x => x.ReservedMinutes)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.AvailableMinutesAtBooking)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Status)
            .InclusiveBetween(1, 4);

        RuleFor(x => x.ReservationReason)
            .NotEmpty()
            .MaximumLength(SchedulingValidationConstants.LongNameMaxLength);
    }
}
