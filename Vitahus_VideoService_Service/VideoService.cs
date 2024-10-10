using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Vitahus_VideoService_Repository;
using Vitahus_VideoService_Service.RabbitMQ;
using Vitahus_VideoService_Shared;

namespace Vitahus_VideoService_Service;

public class VideoService(IGenericRepository<Video> videoCollection, IRabbitMQService rabbitMQService, ILogger<VideoService> logger) : IVideoService

{
    private readonly IGenericRepository<Video> _videoCollection = videoCollection;
    private readonly IRabbitMQService _rabbitMQService = rabbitMQService;
    private readonly ILogger<VideoService> _logger = logger;
    public async Task<Video?> GetVideoAsync(Guid videoId)
    {
        Console.WriteLine($"Starter GetVideoAsync metoden med videoId: {videoId}");

        var auditLog = new AuditLog
        {
            UserId = Guid.NewGuid(),
            Operation = "GetById",
            Collection = "Video",
            DocumentId = videoId,
            Timestamp = DateTimeOffset.UtcNow
        };
        Console.WriteLine($"AuditLog oprettet: {auditLog.ToJson()}");

        var auditLogMessage = auditLog.ToJson();
        Console.WriteLine($"AuditLog konverteret til JSON: {auditLogMessage}");

        try
        {
            Console.WriteLine("Forsøger at sende auditLog besked til MQ...");
            _rabbitMQService.SendMessage("auditLogQueue", auditLogMessage);
            Console.WriteLine("AuditLog besked sendt til MQ");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fejl ved afsendelse af auditLog besked: {ex.Message}");
        }

        Console.WriteLine($"Henter video med ID: {videoId} fra databasen...");
        var video = await _videoCollection.GetByIdAsync(videoId);
        
        if (video != null)
        {
            Console.WriteLine($"Video fundet: {video.ToJson()}");
        }
        else
        {
            Console.WriteLine("Ingen video fundet med det givne ID");
        }

        return video;
    }

    public async Task<IEnumerable<Video>> GetVideosAsync()
    {
       var result = await _videoCollection.GetAllAsync();

    var auditLog = new AuditLog
    {
        UserId = Guid.NewGuid(),
        Operation = "GetAll",
        Collection = "Video",
        DocumentId = Guid.NewGuid(),
        Timestamp = DateTimeOffset.UtcNow
    };
    var auditLogMessage = auditLog.ToJson();
    _rabbitMQService.SendMessage("auditLogQueue", auditLogMessage);
    
    return result;
    }

    public async Task AddVideoAsync(Video video)
    {
        logger.LogInformation($"[>] Starter AddVideoAsync metoden med video: {video.ToJson()}\n");
        
        var videoMessage = video.ToJson();
        logger.LogInformation($"[o] Video konverteret til JSON: {videoMessage}\n");
        
        try
        {
            logger.LogInformation("[>] Forsøger at sende video besked til MQ...\n");
            _rabbitMQService.SendMessage("videoQueue", videoMessage);
            logger.LogInformation($"[>] Video besked sendt til MQ: {videoMessage}\n");
            
            logger.LogInformation("Forsøger at gemme video i databasen...\n");
            await _videoCollection.CreateAsync(video)!;
            logger.LogInformation($"Video gemt i databasen med ID: {video.Id}\n");

            var auditLog = new AuditLog
            {
                UserId = video.UserId,
                Operation = "Create",
                Collection = "Video",
                DocumentId = video.Id,
                Timestamp = DateTimeOffset.UtcNow
            };
            logger.LogInformation($"AuditLog oprettet: {auditLog.ToJson()}\n");
            var auditLogMessage = auditLog.ToJson();
            logger.LogInformation("Forsøger at sende auditLog besked til MQ...\n");
            _rabbitMQService.SendMessage("auditLogQueue", auditLogMessage);
            logger.LogInformation($"AuditLog besked sendt til MQ: {auditLogMessage}\n");
        }
        catch (Exception ex)
        {
            logger.LogError($"Fejl opstået i AddVideoAsync: {ex.Message}\n");
            logger.LogError($"Stacktrace: {ex.StackTrace}\n");
            throw;
        }
        finally
        {
            logger.LogInformation("AddVideoAsync metoden afsluttet\n");
        }
    }

    public async Task UpdateVideoAsync(Video video)
    {
        await _videoCollection.UpdateAsync(video.Id, video);
        var auditLog = new AuditLog
        {
            UserId = video.UserId,
            Operation = "Update",
            Collection = "Video",
            DocumentId = video.Id,
            Timestamp = DateTimeOffset.UtcNow
        };
        var auditLogMessage = auditLog.ToJson();
        _rabbitMQService.SendMessage("auditLogQueue", auditLogMessage);
    }

    public async Task DeleteVideoAsync(Video? video)
    {
        await _videoCollection.DeleteAsync(video!.Id);
        var auditLog = new AuditLog
        {
            UserId = video.UserId,
            Operation = "Delete",
            Collection = "Video",
            DocumentId = video.Id,
            Timestamp = DateTimeOffset.UtcNow
        };
        var auditLogMessage = auditLog.ToJson();
        _rabbitMQService.SendMessage("auditLogQueue", auditLogMessage);
    }
}