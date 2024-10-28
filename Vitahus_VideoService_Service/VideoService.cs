using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Vitahus_VideoService_Repository;
using Vitahus_VideoService_Service.RabbitMQ;
using Vitahus_VideoService_Shared;

namespace Vitahus_VideoService_Service;

public record VideoMessage(Guid VideoId, string Operation, DateTimeOffset Timestamp);

public class VideoService(
    IGenericRepository<Video> videoCollection,
    IRabbitMQService rabbitMqService,
    ILogger<VideoService> logger
) : IVideoService
{
    public async Task<Video?> GetVideoAsync(Guid videoId)
    {
        logger.LogInformation($"Starter GetVideoAsync metoden med videoId: {videoId}\n");
        var auditLog = new AuditLog
        {
            UserId = Guid.NewGuid(),
            Operation = "GetById",
            Collection = "Video",
            DocumentId = videoId,
            Timestamp = DateTimeOffset.UtcNow
        };
        logger.LogInformation($"AuditLog oprettet: {auditLog.ToJson()}\n");
        var videoMessage = new VideoMessage(
            VideoId: videoId,
            Operation: "GetById",
            Timestamp: DateTimeOffset.UtcNow
        );

        var auditLogMessage = auditLog.ToJson();
        logger.LogInformation($"AuditLog konverteret til JSON: {auditLogMessage}\n");

        try
        {
            logger.LogInformation("Forsøger at sende auditLog besked til MQ...\n");
            rabbitMqService.SendMessage("auditLogQueue", auditLogMessage);
            var videoMessageJson = JsonSerializer.Serialize(videoMessage);
            rabbitMqService.SendMessage("videoQueue", videoMessageJson);
            var stopwatch = Stopwatch.StartNew();
            await Task.Delay(1000 + new Random().Next(3000));
            stopwatch.Stop();
            logger.LogInformation(
                $"[TIME] AuditLog besked sendt til MQ efter {stopwatch.ElapsedMilliseconds} ms\n"
            );
            logger.LogInformation("AuditLog besked sendt til MQ\n");
        }
        catch (Exception ex)
        {
            logger.LogError($"Fejl ved afsendelse af auditLog besked: {ex.Message}\n");
        }

        logger.LogInformation($"Henter video med ID: {videoId} fra databasen...\n");
        var video = await videoCollection.GetByIdAsync(videoId);

        if (video != null)
        {
            logger.LogInformation($"Video fundet: {video.ToJson()}\n");
        }
        else
        {
            logger.LogInformation("Ingen video fundet med det givne ID\n");
        }

        return video;
    }

    public async Task<IEnumerable<Video>> GetVideosAsync()
    {
        logger.LogInformation("Starter GetVideosAsync metoden\n");
        var result = await videoCollection.GetAllAsync();
        var videoMessage = new VideoMessage(
            VideoId: Guid.NewGuid(),
            Operation: "GetAll",
            Timestamp: DateTimeOffset.UtcNow
        );
        var auditLog = new AuditLog
        {
            UserId = Guid.NewGuid(),
            Operation = "GetAll",
            Collection = "Video",
            DocumentId = Guid.NewGuid(),
            Timestamp = DateTimeOffset.UtcNow
        };
        var auditLogMessage = auditLog.ToJson();
        logger.LogInformation($"AuditLog konverteret til JSON: {auditLogMessage}\n");
        rabbitMqService.SendMessage("auditLogQueue", auditLogMessage);
        var videoMessageJson = JsonSerializer.Serialize(videoMessage);
        logger.LogInformation($"VideoMessage konverteret til JSON: {videoMessageJson}\n");
        rabbitMqService.SendMessage("videoQueue", videoMessageJson);

        return result;
    }

    public async Task AddVideoAsync(Video video)
    {
        logger.LogInformation($"[>] Starter AddVideoAsync metoden med video: {video.ToJson()}\n");

        // Simulerer en større opgave ved at tilføje en tilfældig ventetid
        await Task.Delay(1000 + new Random().Next(2000));

        var videoMessage = video.ToJson();
        logger.LogInformation($"[o] Video konverteret til JSON: {videoMessage}\n");

        try
        {
            logger.LogInformation("[>] Forsøger at sende video besked til MQ...\n");
            rabbitMqService.SendMessage("videoQueue", videoMessage);
            logger.LogInformation($"[>] Video besked sendt til MQ: {videoMessage}\n");

            logger.LogInformation("Forsøger at gemme video i databasen...\n");
            await videoCollection.CreateAsync(video)!;
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
            rabbitMqService.SendMessage("auditLogQueue", auditLogMessage);
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
        await videoCollection.UpdateAsync(video.Id, video);
        var auditLog = new AuditLog
        {
            UserId = video.UserId,
            Operation = "Update",
            Collection = "Video",
            DocumentId = video.Id,
            Timestamp = DateTimeOffset.UtcNow
        };
        var auditLogMessage = auditLog.ToJson();
        rabbitMqService.SendMessage("auditLogQueue", auditLogMessage);
    }

    public async Task DeleteVideoAsync(Video? video)
    {
        await videoCollection.DeleteAsync(video!.Id);
        var auditLog = new AuditLog
        {
            UserId = video.UserId,
            Operation = "Delete",
            Collection = "Video",
            DocumentId = video.Id,
            Timestamp = DateTimeOffset.UtcNow
        };
        var auditLogMessage = auditLog.ToJson();
        rabbitMqService.SendMessage("auditLogQueue", auditLogMessage);
    }
}

