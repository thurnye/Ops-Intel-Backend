using FluentValidation;
using OperationIntelligence.Core.Models.Scheduling.Requests.Material;
using OperationIntelligence.Core.Validators.Scheduling;

namespace OperationIntelligence.Core.Validators.Scheduling.Material;

public class UpdateScheduleMaterialCheckRequestValidator : AbstractValidator<UpdateScheduleMaterialCheckRequest>
{
    public UpdateScheduleMaterialCheckRequestValidator()
    {
        RuleFor(x => x.RequiredQuantity).GreaterThanOrEqualTo(0);
        RuleFor(x => x.AvailableQuantity).GreaterThanOrEqualTo(0);
        RuleFor(x => x.ReservedQuantity).GreaterThanOrEqualTo(0);
        RuleFor(x => x.ShortageQuantity).GreaterThanOrEqualTo(0);

        RuleFor(x => x.Status)
            .InclusiveBetween(1, 7);

        RuleFor(x => x.Notes)
            .MaximumLength(SchedulingValidationConstants.NotesMaxLength);

        RuleFor(x => x.CheckedAtUtc)
            .NotEmpty();
    }
}
