using Vitahus_ActivityService_Shared;

namespace Vitahus_ActivityService_Service;
public interface IAuditLogService
{
    Task LogAsync(AuditLog auditLog);
}