using FluentValidation;
using OperationIntelligence.Core.Models.Production.Requests;
using OperationIntelligence.DB;

namespace OperationIntelligence.Core.Validators.Production;

public class CreateProductionQualityCheckRequestValidator : AbstractValidator<CreateProductionQualityCheckRequest>
{
    public CreateProductionQualityCheckRequestValidator()
    {
        RuleFor(x => x.ProductionOrderId).NotEmpty();
        RuleFor(x => x.CheckedByUserId).NotEmpty().MaximumLength(450);
        RuleFor(x => x.CheckType).IsInEnum();
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.CheckDate).NotEmpty();
        RuleFor(x => x.ReferenceStandard).MaximumLength(200);
        RuleFor(x => x.Findings).MaximumLength(2000);
        RuleFor(x => x.CorrectiveAction).MaximumLength(2000);
        RuleFor(x => x.Notes).MaximumLength(1000);

        RuleFor(x => x.CorrectiveAction)
            .NotEmpty()
            .When(x => x.Status == QualityCheckStatus.Failed || x.RequiresRework)
            .WithMessage("Corrective action is required when the quality check fails or rework is required.");
    }
}
