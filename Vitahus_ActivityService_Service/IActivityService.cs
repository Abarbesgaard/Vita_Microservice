using Vitahus_ActivityService_Shared;
namespace Vitahus_ActivityService_Service;

public interface IActivityService
{
    Task<Activity> CreateActivityAsync(Activity activityDto);
    Task<Activity> GetByIdAsync(Guid activityId);
    Task<IEnumerable<Activity>> GetAllAsync();
    Task<Activity> UpdateActivityAsync(Guid activityId, Activity activityDto);
    Task DeleteActivityAsync(Guid activityId);
}