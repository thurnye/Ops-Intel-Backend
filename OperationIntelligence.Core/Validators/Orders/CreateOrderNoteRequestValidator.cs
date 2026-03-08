using FluentValidation;

namespace OperationIntelligence.Core;

public class CreateOrderNoteRequestValidator : AbstractValidator<CreateOrderNoteRequest>
{
    public CreateOrderNoteRequestValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty();

        RuleFor(x => x.Note)
            .NotEmpty()
            .MaximumLength(2000);

        RuleFor(x => x.CreatedBy)
            .NotEmpty()
            .MaximumLength(150);
    }
}
