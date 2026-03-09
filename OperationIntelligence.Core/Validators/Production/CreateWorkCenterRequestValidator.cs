using FluentValidation;
using OperationIntelligence.Core.Models.Production.Requests;

namespace OperationIntelligence.Core.Validators.Production;

public class CreateWorkCenterRequestValidator : AbstractValidator<CreateWorkCenterRequest>
{
    public CreateWorkCenterRequestValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.WarehouseId).NotEmpty();
        RuleFor(x => x.CapacityPerDay).GreaterThanOrEqualTo(0);
        RuleFor(x => x.AvailableOperators).GreaterThanOrEqualTo(0);
    }
}
