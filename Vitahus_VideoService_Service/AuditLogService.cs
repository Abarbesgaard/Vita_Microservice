using MongoDB.Driver;
using Vitahus_VideoService_Shared;

namespace Vitahus_VideoService_Service;

public class AuditLogService : IAuditLogService
{
    private readonly IMongoCollection<AuditLog> _auditLogCollection;

    public AuditLogService(IMongoCollection<AuditLog> auditLogCollection)
    {
        _auditLogCollection = auditLogCollection;
    }

    public async Task LogAsync(AuditLog auditLog)
    {
        ArgumentNullException.ThrowIfNull(auditLog);

        await _auditLogCollection.InsertOneAsync(auditLog);
    }
}