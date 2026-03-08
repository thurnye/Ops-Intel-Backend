using FluentValidation;
namespace OperationIntelligence.Core;

public class StockOutRequestValidator : AbstractValidator<StockOutRequest>
{
    public StockOutRequestValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.WarehouseId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}
