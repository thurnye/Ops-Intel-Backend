namespace OperationIntelligence.Core;

public class WorkforceSummaryDto
{
    /// <summary>
    /// Total active employees/operators in the system
    /// </summary>
    public int ActiveStaff { get; set; }

    /// <summary>
    /// Percentage of staffing coverage against required capacity (0–100)
    /// </summary>
    public decimal ShiftCoverage { get; set; }

    /// <summary>
    /// Number of open/unfilled positions
    /// </summary>
    public int OpenPositions { get; set; }
}