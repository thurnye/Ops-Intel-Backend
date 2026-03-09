using FluentValidation;

namespace OperationIntelligence.Core;

public class AddCustomsDocumentRequestValidator : AbstractValidator<AddCustomsDocumentRequest>
{
    public AddCustomsDocumentRequestValidator()
    {
        RuleFor(x => x.DocumentNumber)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.FileName)
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(x => x.FileUrl)
            .NotEmpty()
            .MaximumLength(1000);

        RuleFor(x => x.CountryOfOrigin).MaximumLength(100);
        RuleFor(x => x.DestinationCountry).MaximumLength(100);
        RuleFor(x => x.HarmonizedCode).MaximumLength(50);
        RuleFor(x => x.CurrencyCode).ValidCurrencyCode();
        RuleFor(x => x.Notes).MaximumLength(1000);

        RuleFor(x => x.DeclaredCustomsValue)
            .GreaterThanOrEqualTo(0)
            .When(x => x.DeclaredCustomsValue.HasValue);
    }
}
