using FluentValidation;
using OperationIntelligence.Core.Models.Production.Requests;

namespace OperationIntelligence.Core.Validators.Production;

public class CreateProductionExecutionRequestValidator : AbstractValidator<CreateProductionExecutionRequest>
{
    public CreateProductionExecutionRequestValidator()
    {
        RuleFor(x => x.ProductionOrderId).NotEmpty();
        RuleFor(x => x.WorkCenterId).NotEmpty();
        RuleFor(x => x.PlannedQuantity).GreaterThan(0);
        RuleFor(x => x.PlannedStartDate).NotEmpty();
        RuleFor(x => x.PlannedEndDate).GreaterThanOrEqualTo(x => x.PlannedStartDate);
        RuleFor(x => x.Remarks).MaximumLength(2000);
    }
}
