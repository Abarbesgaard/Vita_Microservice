using AutoMapper;
using Vitahus_ActivityService_Shared;
using Vitahus_ActivityService_Repository;
using Microsoft.Extensions.Logging;

namespace Vitahus_ActivityService_Service;

public class ActivityService(
    IMapper mapper,
    IGenericRepository<Activity> activityRepository,
    IAuditLogService auditLogService,
    ILogger<ActivityService> logger)
    : IActivityService
{
    public async Task<Activity> CreateActivityAsync(Activity activityDto)
    {
        logger.LogInformation("Creating activity");
        var activity = mapper.Map<Activity>(activityDto);
        await activityRepository.CreateAsync(activity)!;
        await auditLogService.LogAsync(new AuditLog
        {
            UserId = Guid.NewGuid(),
            Operation = "Create",
            Collection = "Activity",
            DocumentId = activity.Id,
            Timestamp = DateTimeOffset.Now
        });
        return activity;
    }

    public async Task<Activity> GetByIdAsync(Guid activityId)
    {
        logger.LogInformation("Getting activity by ID: {ActivityId}", activityId);
        await auditLogService.LogAsync(new AuditLog
        {
            UserId = Guid.NewGuid(),
            Operation = "GetById",
            Collection = "Activity",
            DocumentId = activityId,
            Timestamp = DateTimeOffset.Now
        });

        return await activityRepository?.GetByIdAsync(activityId)! ?? throw new Exception("Activity not found");
    }

    public async Task<IEnumerable<Activity>> GetAllAsync()
    {
        logger.LogInformation("Getting all activities");
        await auditLogService.LogAsync(new AuditLog
        {
            UserId = Guid.NewGuid(),
            Operation = "GetAll",
            Collection = "Activity",
            DocumentId = Guid.NewGuid(),
            Timestamp = DateTimeOffset.Now
        });

        return await activityRepository.GetAllAsync() ?? throw new Exception("No activities found");
    }

    public async Task<Activity> UpdateActivityAsync(Guid activityId, Activity activityDto)
    {
        logger.LogInformation("Updating activity with ID: {ActivityId}", activityId);
        var activity = await activityRepository.GetByIdAsync(activityId);
        if (activity == null)
        {
            logger.LogError("Activity not found");
            throw new Exception("Activity not found");
        }

        mapper.Map(activityDto, activity);
        await activityRepository.UpdateAsync(activityId, activity);
        logger.LogInformation("Activity updated successfully");
        await auditLogService.LogAsync(new AuditLog
        {
            UserId = Guid.NewGuid(),
            Operation = "Update",
            Collection = "Activity",
            DocumentId = activity.Id,
            Timestamp = DateTimeOffset.Now
        });
        return activity;
    }

    public async Task DeleteActivityAsync(Guid activityId)
    {
        logger.LogInformation("Deleting activity with ID: {ActivityId}", activityId);
        var activity = await activityRepository.GetByIdAsync(activityId);
        if (activity == null)
        {
            logger.LogError("Activity not found");
            throw new Exception("Activity not found");
        }

        await activityRepository.DeleteAsync(activityId);
        logger.LogInformation("Activity deleted successfully");
        await auditLogService.LogAsync(new AuditLog
        {
            UserId = Guid.NewGuid(),
            Operation = "Delete",
            Collection = "Activity",
            DocumentId = activity.Id,
            Timestamp = DateTimeOffset.Now
        });
    }
}
