using Microsoft.AspNetCore.Mvc;
using OperationIntelligence.Api.Models;
using OperationIntelligence.Core;

namespace OperationIntelligence.Api.Controllers.Shipment;

[Route("api/shipments")]
public class ShipmentsController : BaseApiController
{
    private readonly IShipmentService _shipmentService;

    public ShipmentsController(IShipmentService shipmentService)
    {
        _shipmentService = shipmentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged(
        [FromQuery] ShipmentFilterRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _shipmentService.GetPagedAsync(request, cancellationToken);
        return PagedOkResponse(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _shipmentService.GetByIdAsync(id, cancellationToken);
        if (result == null)
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, "Shipment not found.");

        return OkResponse(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateShipmentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _shipmentService.CreateAsync(request, User?.Identity?.Name, cancellationToken);
        return CreatedResponse(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateShipmentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _shipmentService.UpdateAsync(id, request, User?.Identity?.Name, cancellationToken);
        return OkResponse(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _shipmentService.DeleteAsync(id, User?.Identity?.Name, cancellationToken);
        if (!deleted)
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, "Shipment not found.");

        return OkResponse(new { Message = "Shipment deleted successfully." });
    }

    [HttpGet("{shipmentId:guid}/items")]
    public async Task<IActionResult> GetItems(Guid shipmentId, CancellationToken cancellationToken)
    {
        var result = await _shipmentService.GetItemsAsync(shipmentId, cancellationToken);
        return OkResponse(result);
    }

    [HttpPost("{shipmentId:guid}/items")]
    public async Task<IActionResult> AddItem(
        Guid shipmentId,
        [FromBody] AddShipmentItemRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _shipmentService.AddItemAsync(shipmentId, request, User?.Identity?.Name, cancellationToken);
        return CreatedResponse(result);
    }

    [HttpPut("{shipmentId:guid}/items/{shipmentItemId:guid}")]
    public async Task<IActionResult> UpdateItem(
        Guid shipmentId,
        Guid shipmentItemId,
        [FromBody] UpdateShipmentItemRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _shipmentService.UpdateItemAsync(shipmentId, shipmentItemId, request, User?.Identity?.Name, cancellationToken);
        return OkResponse(result);
    }

    [HttpDelete("{shipmentId:guid}/items/{shipmentItemId:guid}")]
    public async Task<IActionResult> RemoveItem(
        Guid shipmentId,
        Guid shipmentItemId,
        CancellationToken cancellationToken)
    {
        var deleted = await _shipmentService.RemoveItemAsync(shipmentId, shipmentItemId, User?.Identity?.Name, cancellationToken);
        if (!deleted)
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, "Shipment item not found.");

        return OkResponse(new { Message = "Shipment item removed successfully." });
    }

    [HttpGet("{shipmentId:guid}/packages")]
    public async Task<IActionResult> GetPackages(Guid shipmentId, CancellationToken cancellationToken)
    {
        var result = await _shipmentService.GetPackagesAsync(shipmentId, cancellationToken);
        return OkResponse(result);
    }

    [HttpPost("{shipmentId:guid}/packages")]
    public async Task<IActionResult> AddPackage(
        Guid shipmentId,
        [FromBody] AddShipmentPackageRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _shipmentService.AddPackageAsync(shipmentId, request, User?.Identity?.Name, cancellationToken);
        return CreatedResponse(result);
    }

    [HttpPut("{shipmentId:guid}/packages/{shipmentPackageId:guid}")]
    public async Task<IActionResult> UpdatePackage(
        Guid shipmentId,
        Guid shipmentPackageId,
        [FromBody] UpdateShipmentPackageRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _shipmentService.UpdatePackageAsync(shipmentId, shipmentPackageId, request, User?.Identity?.Name, cancellationToken);
        return OkResponse(result);
    }

    [HttpDelete("{shipmentId:guid}/packages/{shipmentPackageId:guid}")]
    public async Task<IActionResult> RemovePackage(
        Guid shipmentId,
        Guid shipmentPackageId,
        CancellationToken cancellationToken)
    {
        var deleted = await _shipmentService.RemovePackageAsync(shipmentId, shipmentPackageId, User?.Identity?.Name, cancellationToken);
        if (!deleted)
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, "Shipment package not found.");

        return OkResponse(new { Message = "Shipment package removed successfully." });
    }

    [HttpPost("{shipmentId:guid}/packages/{shipmentPackageId:guid}/items")]
    public async Task<IActionResult> AddPackageItem(
        Guid shipmentId,
        Guid shipmentPackageId,
        [FromBody] AddShipmentPackageItemRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _shipmentService.AddPackageItemAsync(shipmentId, shipmentPackageId, request, User?.Identity?.Name, cancellationToken);
        return CreatedResponse(result);
    }

    [HttpDelete("{shipmentId:guid}/packages/{shipmentPackageId:guid}/items/{shipmentPackageItemId:guid}")]
    public async Task<IActionResult> RemovePackageItem(
        Guid shipmentId,
        Guid shipmentPackageId,
        Guid shipmentPackageItemId,
        CancellationToken cancellationToken)
    {
        var deleted = await _shipmentService.RemovePackageItemAsync(
            shipmentId,
            shipmentPackageId,
            shipmentPackageItemId,
            User?.Identity?.Name,
            cancellationToken);

        if (!deleted)
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, "Shipment package item not found.");

        return OkResponse(new { Message = "Shipment package item removed successfully." });
    }

    [HttpGet("{shipmentId:guid}/tracking-events")]
    public async Task<IActionResult> GetTrackingEvents(Guid shipmentId, CancellationToken cancellationToken)
    {
        var result = await _shipmentService.GetTrackingEventsAsync(shipmentId, cancellationToken);
        return OkResponse(result);
    }

    [HttpPost("{shipmentId:guid}/tracking-events")]
    public async Task<IActionResult> AddTrackingEvent(
        Guid shipmentId,
        [FromBody] AddShipmentTrackingEventRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _shipmentService.AddTrackingEventAsync(shipmentId, request, User?.Identity?.Name, cancellationToken);
        return CreatedResponse(result);
    }

    [HttpGet("{shipmentId:guid}/documents")]
    public async Task<IActionResult> GetDocuments(Guid shipmentId, CancellationToken cancellationToken)
    {
        var result = await _shipmentService.GetDocumentsAsync(shipmentId, cancellationToken);
        return OkResponse(result);
    }

    [HttpPost("{shipmentId:guid}/documents")]
    public async Task<IActionResult> AddDocument(
        Guid shipmentId,
        [FromBody] AddShipmentDocumentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _shipmentService.AddDocumentAsync(shipmentId, request, User?.Identity?.Name, cancellationToken);
        return CreatedResponse(result);
    }

    [HttpGet("{shipmentId:guid}/charges")]
    public async Task<IActionResult> GetCharges(Guid shipmentId, CancellationToken cancellationToken)
    {
        var result = await _shipmentService.GetChargesAsync(shipmentId, cancellationToken);
        return OkResponse(result);
    }

    [HttpPost("{shipmentId:guid}/charges")]
    public async Task<IActionResult> AddCharge(
        Guid shipmentId,
        [FromBody] AddShipmentChargeRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _shipmentService.AddChargeAsync(shipmentId, request, User?.Identity?.Name, cancellationToken);
        return CreatedResponse(result);
    }

    [HttpGet("{shipmentId:guid}/exceptions")]
    public async Task<IActionResult> GetExceptions(Guid shipmentId, CancellationToken cancellationToken)
    {
        var result = await _shipmentService.GetExceptionsAsync(shipmentId, cancellationToken);
        return OkResponse(result);
    }

    [HttpPost("{shipmentId:guid}/exceptions")]
    public async Task<IActionResult> AddException(
        Guid shipmentId,
        [FromBody] AddShipmentExceptionRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _shipmentService.AddExceptionAsync(shipmentId, request, User?.Identity?.Name, cancellationToken);
        return CreatedResponse(result);
    }

    [HttpGet("{shipmentId:guid}/insurances")]
    public async Task<IActionResult> GetInsurances(Guid shipmentId, CancellationToken cancellationToken)
    {
        var result = await _shipmentService.GetInsurancesAsync(shipmentId, cancellationToken);
        return OkResponse(result);
    }

    [HttpPost("{shipmentId:guid}/insurances")]
    public async Task<IActionResult> AddInsurance(
        Guid shipmentId,
        [FromBody] AddShipmentInsuranceRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _shipmentService.AddInsuranceAsync(shipmentId, request, User?.Identity?.Name, cancellationToken);
        return CreatedResponse(result);
    }

    [HttpGet("{shipmentId:guid}/status-history")]
    public async Task<IActionResult> GetStatusHistory(Guid shipmentId, CancellationToken cancellationToken)
    {
        var result = await _shipmentService.GetStatusHistoryAsync(shipmentId, cancellationToken);
        return OkResponse(result);
    }

    [HttpGet("{shipmentId:guid}/customs-documents")]
    public async Task<IActionResult> GetCustomsDocuments(Guid shipmentId, CancellationToken cancellationToken)
    {
        var result = await _shipmentService.GetCustomsDocumentsAsync(shipmentId, cancellationToken);
        return OkResponse(result);
    }

    [HttpPost("{shipmentId:guid}/customs-documents")]
    public async Task<IActionResult> AddCustomsDocument(
        Guid shipmentId,
        [FromBody] AddCustomsDocumentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _shipmentService.AddCustomsDocumentAsync(shipmentId, request, User?.Identity?.Name, cancellationToken);
        return CreatedResponse(result);
    }

    [HttpPut("{shipmentId:guid}/status")]
    public async Task<IActionResult> UpdateStatus(
        Guid shipmentId,
        [FromBody] UpdateShipmentStatusRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _shipmentService.UpdateStatusAsync(shipmentId, request, User?.Identity?.Name, cancellationToken);
        return OkResponse(result);
    }

    [HttpPut("{shipmentId:guid}/assign-delivery-run")]
    public async Task<IActionResult> AssignToDeliveryRun(
        Guid shipmentId,
        [FromBody] AssignShipmentToDeliveryRunRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _shipmentService.AssignToDeliveryRunAsync(shipmentId, request, User?.Identity?.Name, cancellationToken);
        return OkResponse(result);
    }

    [HttpPut("{shipmentId:guid}/unassign-delivery-run")]
    public async Task<IActionResult> UnassignFromDeliveryRun(Guid shipmentId, CancellationToken cancellationToken)
    {
        var result = await _shipmentService.UnassignFromDeliveryRunAsync(shipmentId, User?.Identity?.Name, cancellationToken);
        return OkResponse(result);
    }

    [HttpPut("{shipmentId:guid}/assign-dock-appointment")]
    public async Task<IActionResult> AssignToDockAppointment(
        Guid shipmentId,
        [FromBody] AssignShipmentToDockAppointmentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _shipmentService.AssignToDockAppointmentAsync(shipmentId, request, User?.Identity?.Name, cancellationToken);
        return OkResponse(result);
    }

    [HttpPut("{shipmentId:guid}/unassign-dock-appointment")]
    public async Task<IActionResult> UnassignFromDockAppointment(Guid shipmentId, CancellationToken cancellationToken)
    {
        var result = await _shipmentService.UnassignFromDockAppointmentAsync(shipmentId, User?.Identity?.Name, cancellationToken);
        return OkResponse(result);
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary(
        [FromQuery] ShipmentFilterRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _shipmentService.GetSummaryAsync(request, cancellationToken);
        return OkResponse(result);
    }
}