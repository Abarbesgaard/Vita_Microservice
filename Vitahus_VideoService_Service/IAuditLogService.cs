using Vitahus_VideoService_Shared;

namespace Vitahus_VideoService_Service;

public interface IAuditLogService
{
    Task LogAsync(AuditLog auditLog);
}
