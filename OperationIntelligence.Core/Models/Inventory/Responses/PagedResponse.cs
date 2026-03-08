namespace OperationIntelligence.Core;

public class PagedResponse<T>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalRecords { get; set; }
    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling((double)TotalRecords / PageSize);
    public IReadOnlyList<T> Items { get; set; } = new List<T>();
}
