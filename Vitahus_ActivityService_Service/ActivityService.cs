using AutoMapper;
using Vitahus_ActivityService_Shared;
using Vitahus_ActivityService_Repository;

namespace Vitahus_ActivityService_Service;

public class ActivityService : IActivityService
{
    private readonly IMapper _mapper;
    private readonly IGenericRepository<Activity> _activityRepository;
    private readonly IAuditLogService _auditLogService;

    public ActivityService(IMapper mapper, IGenericRepository<Activity> activityRepository, IAuditLogService auditLogService)
    {
        _mapper = mapper;
        _activityRepository = activityRepository;
        _auditLogService = auditLogService;
    }

    public async Task<Activity> CreateActivityAsync(Activity activityDto)
    {
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
        return await _activityRepository?.GetByIdAsync(activityId)! ?? throw new Exception("Activity not found");
    }

    public async Task<IEnumerable<Activity>> GetAllAsync()
    {
        return await _activityRepository.GetAllAsync();
    }

    public async Task<Activity> UpdateActivityAsync(Guid activityId, Activity activityDto)
    {
        var activity = await _activityRepository.GetByIdAsync(activityId);
        if (activity == null)
        {
            throw new Exception("Activity not found");
        }

        _mapper.Map(activityDto, activity);
        await _activityRepository.UpdateAsync(activityId, activity);
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
        var activity = await _activityRepository.GetByIdAsync(activityId);
        if (activity == null)
        {
            throw new Exception("Activity not found");
        }

        await _activityRepository.DeleteAsync(activityId);
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