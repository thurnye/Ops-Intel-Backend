using FluentValidation;
using OperationIntelligence.Core.Models.Production.Requests;

namespace OperationIntelligence.Core.Validators.Production;

public class CreateProductionMaterialConsumptionRequestValidator : AbstractValidator<CreateProductionMaterialConsumptionRequest>
{
    public CreateProductionMaterialConsumptionRequestValidator()
    {
        RuleFor(x => x.ProductionMaterialIssueId).NotEmpty();
        RuleFor(x => x.ConsumedQuantity).GreaterThan(0);
        RuleFor(x => x.ConsumptionDate).NotEmpty();
        RuleFor(x => x.Notes).MaximumLength(1000);
    }
}
