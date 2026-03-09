using FluentValidation;
using OperationIntelligence.Core.Models.Production.Requests;

namespace OperationIntelligence.Core.Validators.Production;

public class CreateProductionMaterialIssueRequestValidator : AbstractValidator<CreateProductionMaterialIssueRequest>
{
    public CreateProductionMaterialIssueRequestValidator()
    {
        RuleFor(x => x.ProductionOrderId).NotEmpty();
        RuleFor(x => x.MaterialProductId).NotEmpty();
        RuleFor(x => x.WarehouseId).NotEmpty();
        RuleFor(x => x.UnitOfMeasureId).NotEmpty();
        RuleFor(x => x.PlannedQuantity).GreaterThanOrEqualTo(0);
        RuleFor(x => x.IssuedQuantity).GreaterThan(0);
        RuleFor(x => x.BatchNumber).MaximumLength(100);
        RuleFor(x => x.LotNumber).MaximumLength(100);
        RuleFor(x => x.IssueDate).NotEmpty();
        RuleFor(x => x.Notes).MaximumLength(1000);
    }
}
