using FluentValidation;

namespace OperationIntelligence.Core;

public class CreateOrderImageRequestValidator : AbstractValidator<CreateOrderImageRequest>
{
    private static readonly string[] AllowedContentTypes =
    [
        "image/jpeg",
        "image/png",
        "image/webp",
        "application/pdf"
    ];

    public CreateOrderImageRequestValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty();

        RuleFor(x => x.FileName)
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(x => x.OriginalFileName)
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(x => x.FileExtension)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(x => x.ContentType)
            .NotEmpty()
            .MaximumLength(100)
            .Must(x => AllowedContentTypes.Contains(x))
            .WithMessage(OrderValidationMessages.UnsupportedFileType);

        RuleFor(x => x.FileSizeBytes)
            .GreaterThan(0)
            .LessThanOrEqualTo(10 * 1024 * 1024);

        RuleFor(x => x.StoragePath)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x.PublicUrl)
            .MaximumLength(1000);

        RuleFor(x => x.ImageType)
            .IsInEnum();

        RuleFor(x => x.Caption)
            .MaximumLength(500);

        RuleFor(x => x.UploadedBy)
            .NotEmpty()
            .MaximumLength(150);
    }
}
