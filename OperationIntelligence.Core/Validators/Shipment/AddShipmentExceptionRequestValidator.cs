using FluentValidation;

namespace OperationIntelligence.Core;

public class AddShipmentExceptionRequestValidator : AbstractValidator<AddShipmentExceptionRequest>
{
    public AddShipmentExceptionRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description).MaximumLength(1000);
        RuleFor(x => x.ReportedBy).MaximumLength(150);
    }
}
