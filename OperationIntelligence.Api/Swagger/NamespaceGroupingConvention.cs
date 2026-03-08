using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace OperationIntelligence.Api.Swagger;

public sealed class NamespaceGroupingConvention : IControllerModelConvention
{
    public void Apply(ControllerModel controller)
    {
        var ns = controller.ControllerType.Namespace;

        if (string.IsNullOrWhiteSpace(ns))
        {
            controller.ApiExplorer.GroupName = "default";
            return;
        }

        var marker = ns.Contains(".Controllers.", StringComparison.OrdinalIgnoreCase)
            ? ".Controllers."
            : ".Controller.";
        var index = ns.IndexOf(marker, StringComparison.OrdinalIgnoreCase);

        var group = index < 0
            ? null
            : ns[(index + marker.Length)..]
            .Split('.', StringSplitOptions.RemoveEmptyEntries)
            .FirstOrDefault();

        controller.ApiExplorer.GroupName = string.IsNullOrWhiteSpace(group)
            ? "default"
            : group.ToLowerInvariant();
    }
}
