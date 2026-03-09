namespace OperationIntelligence.Core;

public static class SchedulingErrorMessages
{
    public const string SchedulePlanNumberAlreadyExists = "Schedule plan number already exists.";
    public const string SchedulePlanNotFound = "Schedule plan not found.";
    public const string SourceSchedulePlanNotFound = "Source schedule plan not found.";
    public const string NewSchedulePlanNumberAlreadyExists = "New schedule plan number already exists.";

    public const string ScheduleJobNumberAlreadyExists = "Schedule job number already exists.";
    public const string ScheduleJobNotFound = "Schedule job not found.";
    public const string ScheduleJobCannotBeReleasedWhileOnQualityHold = "Schedule job cannot be released while on quality hold.";

    public const string ScheduleOperationNotFound = "Schedule operation not found.";
    public const string OverlappingWorkCenterOperationDetected = "Overlapping work center operation detected.";
    public const string OverlappingMachineOperationDetected = "Overlapping machine operation detected.";
    public const string DependencyAlreadyExists = "Dependency already exists.";
    public const string ConstraintNotFound = "Constraint not found.";
    public const string OperationDependencyNotFound = "Operation dependency not found.";
    public const string OperationConstraintNotFound = "Operation constraint not found.";
    public const string OperationResourceOptionNotFound = "Operation resource option not found.";
    public const string ResourceAssignmentNotFound = "Resource assignment not found.";
    public const string OverlappingResourceAssignmentDetected = "Overlapping resource assignment detected.";

    public const string ShiftCodeAlreadyExistsInWarehouse = "Shift code already exists in this warehouse.";
    public const string ShiftNotFound = "Shift not found.";

    public const string ResourceCalendarNotFound = "Resource calendar not found.";
    public const string ResourceCalendarExceptionNotFound = "Resource calendar exception not found.";

    public const string OverlappingCapacityReservationDetected = "Overlapping capacity reservation detected.";
    public const string CapacityReservationNotFound = "Capacity reservation not found.";

    public const string QueuePositionAlreadyExists = "Queue position already exists.";
    public const string DispatchQueueItemNotFound = "Dispatch queue item not found.";

    public const string ScheduleMaterialCheckNotFound = "Schedule material check not found.";

    public const string ScheduleExceptionNotFound = "Schedule exception not found.";

    public const string RevisionNumberAlreadyExistsForSchedulePlan = "Revision number already exists for this schedule plan.";
    public const string ScheduleRevisionNotFound = "Schedule revision not found.";
    public const string ScheduleRescheduleHistoryNotFound = "Schedule reschedule history not found.";
    public const string ScheduleStatusHistoryNotFound = "Schedule status history not found.";
    public const string ScheduleAuditLogNotFound = "Schedule audit log not found.";

    public const string SchedulePlanDeletedSuccessfully = "Schedule plan deleted successfully.";
    public const string ScheduleJobDeletedSuccessfully = "Schedule job deleted successfully.";
    public const string ScheduleOperationDeletedSuccessfully = "Schedule operation deleted successfully.";
    public const string ShiftDeletedSuccessfully = "Shift deleted successfully.";
    public const string ResourceCalendarDeletedSuccessfully = "Resource calendar deleted successfully.";
    public const string ResourceCalendarExceptionDeletedSuccessfully = "Resource calendar exception deleted successfully.";
    public const string CapacityReservationDeletedSuccessfully = "Capacity reservation deleted successfully.";
    public const string DispatchQueueItemDeletedSuccessfully = "Dispatch queue item deleted successfully.";
    public const string ScheduleMaterialCheckDeletedSuccessfully = "Schedule material check deleted successfully.";
    public const string ScheduleExceptionDeletedSuccessfully = "Schedule exception deleted successfully.";
    public const string OperationDependencyDeletedSuccessfully = "Operation dependency deleted successfully.";
    public const string OperationConstraintDeletedSuccessfully = "Operation constraint deleted successfully.";
    public const string OperationResourceOptionDeletedSuccessfully = "Operation resource option deleted successfully.";
    public const string ResourceAssignmentDeletedSuccessfully = "Resource assignment deleted successfully.";
}
