using MongoDB.Driver;
using Vitahus_VideoService_Repository;
using Vitahus_VideoService_Shared;

namespace Vitahus_VideoService_Service;

public class VideoService : IVideoService
{
    private readonly IGenericRepository<Video> _videoCollection;
    private readonly IMongoCollection<AuditLog> _auditLogCollection;

    public VideoService(IGenericRepository<Video> videoCollection, IMongoCollection<AuditLog> auditLogCollection)
    {
        _videoCollection = videoCollection;
        _auditLogCollection = auditLogCollection;
    }

    public async Task<Video?> GetVideoAsync(Guid videoId)
    {
        await _auditLogCollection.InsertOneAsync(new AuditLog
        {
            UserId = Guid.NewGuid(),
            Operation = "GetById",
            Collection = "Video",
            DocumentId = videoId,
            Timestamp = DateTimeOffset.UtcNow
        });
        return await _videoCollection.GetByIdAsync(videoId);
    }

    public async Task<IEnumerable<Video>> GetVideosAsync()
    {
        await _auditLogCollection.InsertOneAsync(new AuditLog
        {
            UserId = Guid.NewGuid(),
            Operation = "GetAll",
            Collection = "Video",
            DocumentId = Guid.NewGuid(),
            Timestamp = DateTimeOffset.UtcNow
        });
        return await _videoCollection.GetAllAsync();
    }

    public async Task AddVideoAsync(Video video)
    {
        await _videoCollection.CreateAsync(video)!;
        await _auditLogCollection.InsertOneAsync(new AuditLog
        {
            UserId = video.UserId,
            Operation = "Create",
            Collection = "Video",
            DocumentId = video.Id,
            Timestamp = DateTimeOffset.UtcNow
        });
    }

    public async Task UpdateVideoAsync(Video video)
    {
        await _videoCollection.UpdateAsync(video.Id, video);
        await _auditLogCollection.InsertOneAsync(new AuditLog
        {
            UserId = video.UserId,
            Operation = "Update",
            Collection = "Video",
            DocumentId = video.Id,
            Timestamp = DateTimeOffset.UtcNow
        });
    }

    public async Task DeleteVideoAsync(Video? video)
    {
        await _videoCollection.DeleteAsync(video!.Id);
        await _auditLogCollection.InsertOneAsync(new AuditLog
        {
            UserId = video.UserId,
            Operation = "Delete",
            Collection = "Video",
            DocumentId = video.Id,
            Timestamp = DateTimeOffset.UtcNow
        });
    }
}