namespace Vitahus_ActivityService_Shared;

public class AuditLog
{
    public Guid UserId { get; set; }
    public string? Operation { get; set; }
    public string? Collection { get; set; }
    public Guid? DocumentId { get; set; }
    public DateTimeOffset Timestamp { get; set; }
}