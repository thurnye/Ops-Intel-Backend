using FluentValidation;
using OperationIntelligence.Core.Models.Scheduling.Requests.Shift;
using OperationIntelligence.Core.Validators.Scheduling;

namespace OperationIntelligence.Core.Validators.Scheduling.Shift;

public class CreateShiftRequestValidator : AbstractValidator<CreateShiftRequest>
{
    public CreateShiftRequestValidator()
    {
        RuleFor(x => x.WarehouseId).NotEmpty();

        RuleFor(x => x.ShiftCode)
            .NotEmpty()
            .MaximumLength(30);

        RuleFor(x => x.ShiftName)
            .NotEmpty()
            .MaximumLength(SchedulingValidationConstants.NameMaxLength);

        RuleFor(x => x.CapacityMinutes)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.BreakMinutes)
            .GreaterThanOrEqualTo(0);
    }
}
