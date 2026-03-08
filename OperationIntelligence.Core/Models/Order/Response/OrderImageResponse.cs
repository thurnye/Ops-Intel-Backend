namespace OperationIntelligence.Core;

public class OrderImageResponse
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public string FileName { get; set; } = default!;
    public string? PublicUrl { get; set; }
    public OrderImageType ImageType { get; set; }
    public string? Caption { get; set; }
    public bool IsPrimary { get; set; }
}