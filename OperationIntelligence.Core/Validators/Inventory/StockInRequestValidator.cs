using FluentValidation;
namespace OperationIntelligence.Core;

public class StockInRequestValidator : AbstractValidator<StockInRequest>
{
    public StockInRequestValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.WarehouseId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}
