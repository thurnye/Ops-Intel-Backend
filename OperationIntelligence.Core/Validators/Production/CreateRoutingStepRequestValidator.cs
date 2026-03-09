using FluentValidation;
using OperationIntelligence.Core.Models.Production.Requests;

namespace OperationIntelligence.Core.Validators.Production;

public class CreateRoutingStepRequestValidator : AbstractValidator<CreateRoutingStepRequest>
{
    public CreateRoutingStepRequestValidator()
    {
        RuleFor(x => x.RoutingId).NotEmpty();
        RuleFor(x => x.Sequence).GreaterThan(0);
        RuleFor(x => x.OperationCode).NotEmpty().MaximumLength(50);
        RuleFor(x => x.OperationName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.WorkCenterId).NotEmpty();
        RuleFor(x => x.SetupTimeMinutes).GreaterThanOrEqualTo(0);
        RuleFor(x => x.RunTimeMinutesPerUnit).GreaterThanOrEqualTo(0);
        RuleFor(x => x.QueueTimeMinutes).GreaterThanOrEqualTo(0);
        RuleFor(x => x.WaitTimeMinutes).GreaterThanOrEqualTo(0);
        RuleFor(x => x.MoveTimeMinutes).GreaterThanOrEqualTo(0);
        RuleFor(x => x.RequiredOperators).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Instructions).MaximumLength(2000);
        RuleFor(x => x.Notes).MaximumLength(1000);
    }
}
