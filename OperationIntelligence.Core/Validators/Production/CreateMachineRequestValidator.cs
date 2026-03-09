using FluentValidation;
using OperationIntelligence.Core.Models.Production.Requests;

namespace OperationIntelligence.Core.Validators.Production;

public class CreateMachineRequestValidator : AbstractValidator<CreateMachineRequest>
{
    public CreateMachineRequestValidator()
    {
        RuleFor(x => x.MachineCode).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.WorkCenterId).NotEmpty();
        RuleFor(x => x.Model).MaximumLength(100);
        RuleFor(x => x.Manufacturer).MaximumLength(100);
        RuleFor(x => x.SerialNumber).MaximumLength(100);
        RuleFor(x => x.HourlyRunningCost).GreaterThanOrEqualTo(0);
        RuleFor(x => x.NextMaintenanceDate)
            .GreaterThanOrEqualTo(x => x.LastMaintenanceDate!.Value)
            .When(x => x.LastMaintenanceDate.HasValue && x.NextMaintenanceDate.HasValue);
    }
}
