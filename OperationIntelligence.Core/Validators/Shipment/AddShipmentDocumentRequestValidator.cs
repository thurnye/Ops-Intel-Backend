using FluentValidation;

namespace OperationIntelligence.Core;

public class AddShipmentDocumentRequestValidator : AbstractValidator<AddShipmentDocumentRequest>
{
    public AddShipmentDocumentRequestValidator()
    {
        RuleFor(x => x.FileName)
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(x => x.FileUrl)
            .NotEmpty()
            .MaximumLength(1000);

        RuleFor(x => x.ContentType).MaximumLength(100);
        RuleFor(x => x.FileSizeBytes).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Notes).MaximumLength(1000);
    }
}
