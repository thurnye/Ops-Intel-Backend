using FluentValidation;

namespace OperationIntelligence.Core;

public static class ShipmentValidationRules
{
    public static IRuleBuilderOptions<T, string> ValidCurrencyCode<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty()
            .Length(3, 10);
    }

    public static IRuleBuilderOptions<T, string> OptionalCode<T>(this IRuleBuilder<T, string> ruleBuilder, int maxLength = 100)
    {
        return ruleBuilder
            .MaximumLength(maxLength);
    }

    public static IRuleBuilderOptions<T, string> RequiredCode<T>(this IRuleBuilder<T, string> ruleBuilder, int maxLength = 100)
    {
        return ruleBuilder
            .NotEmpty()
            .MaximumLength(maxLength);
    }

    public static IRuleBuilderOptions<T, decimal> NonNegativeAmount<T>(this IRuleBuilder<T, decimal> ruleBuilder)
    {
        return ruleBuilder.GreaterThanOrEqualTo(0);
    }

    public static IRuleBuilderOptions<T, decimal> PositiveAmount<T>(this IRuleBuilder<T, decimal> ruleBuilder)
    {
        return ruleBuilder.GreaterThan(0);
    }

    public static IRuleBuilderOptions<T, int> ValidPageNumber<T>(this IRuleBuilder<T, int> ruleBuilder)
    {
        return ruleBuilder.GreaterThan(0);
    }

    public static IRuleBuilderOptions<T, int> ValidPageSize<T>(this IRuleBuilder<T, int> ruleBuilder)
    {
        return ruleBuilder.InclusiveBetween(1, 200);
    }
}
