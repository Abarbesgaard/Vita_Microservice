using AutoMapper;
using Vitahus_ActivityService_Shared;
using Vitahus_ActivityService_Repository;
using Microsoft.Extensions.Logging;

namespace Vitahus_ActivityService_Service;

public class ActivityService(IMapper mapper, IGenericRepository<Activity> activityRepository, IAuditLogService auditLogService, ILogger<ActivityService> logger) : IActivityService
{
    private readonly IMapper _mapper = mapper;
    private readonly IGenericRepository<Activity> _activityRepository = activityRepository;
    private readonly IAuditLogService _auditLogService = auditLogService;

    private readonly ILogger<ActivityService> _logger = logger;

    public async Task<Activity> CreateActivityAsync(Activity activityDto)
    {
        _logger.LogInformation("Creating activity");
        var activity = _mapper.Map<Activity>(activityDto);
        await _activityRepository.CreateAsync(activity)!;
        await _auditLogService.LogAsync(new AuditLog
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
        _logger.LogInformation("Getting activity by ID: {ActivityId}", activityId);
        await _auditLogService.LogAsync(new AuditLog
        {
            UserId = Guid.NewGuid(),
            Operation = "GetById",
            Collection = "Activity",
            DocumentId = activityId,
            Timestamp = DateTimeOffset.Now
        });

        return await _activityRepository?.GetByIdAsync(activityId)! ?? throw new Exception("Activity not found");
    }

    public async Task<IEnumerable<Activity>> GetAllAsync()
    {
        _logger.LogInformation("Getting all activities");
        await _auditLogService.LogAsync(new AuditLog
        {
            UserId = Guid.NewGuid(),
            Operation = "GetAll",
            Collection = "Activity",
            DocumentId = Guid.NewGuid(),
            Timestamp = DateTimeOffset.Now
        });

        return await _activityRepository.GetAllAsync() ?? throw new Exception("No activities found");
    }

    public async Task<Activity> UpdateActivityAsync(Guid activityId, Activity activityDto)
    {
        _logger.LogInformation("Updating activity with ID: {ActivityId}", activityId);
        var activity = await _activityRepository.GetByIdAsync(activityId);
        if (activity == null)
        {
            _logger.LogError("Activity not found");
            throw new Exception("Activity not found");
        }

        _mapper.Map(activityDto, activity);
        await _activityRepository.UpdateAsync(activityId, activity);
        _logger.LogInformation("Activity updated successfully");
        await _auditLogService.LogAsync(new AuditLog
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
        _logger.LogInformation("Deleting activity with ID: {ActivityId}", activityId);
        var activity = await _activityRepository.GetByIdAsync(activityId);
        if (activity == null)
        {
            _logger.LogError("Activity not found");
            throw new Exception("Activity not found");
        }

        await _activityRepository.DeleteAsync(activityId);
        _logger.LogInformation("Activity deleted successfully");
        await _auditLogService.LogAsync(new AuditLog
        {
            UserId = Guid.NewGuid(),
            Operation = "Delete",
            Collection = "Activity",
            DocumentId = activity.Id,
            Timestamp = DateTimeOffset.Now
        });
    }
}