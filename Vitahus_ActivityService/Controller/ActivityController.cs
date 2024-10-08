using Microsoft.AspNetCore.Mvc;
using Vitahus_ActivityService_Service;
using Vitahus_ActivityService_Shared;

namespace Vitahus_ActivityService.Controller;

public class ActivityController : ControllerBase
{
    private readonly IActivityService _activityService;

    public ActivityController(IActivityService activityService)
    {
        _activityService = activityService;
    }

    [HttpPost("/Create")]
    public async Task<IActionResult> CreateActivity([FromBody] Activity activity)
    {
        var createdActivity = await _activityService.CreateActivityAsync(activity);
        return CreatedAtAction(nameof(GetActivity), new { id = createdActivity.Id }, createdActivity);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllActivities()
    {
        var activities = await _activityService.GetAllAsync();
        return Ok(activities);
    }

    [HttpGet("/GetById/{id}")]
    public async Task<IActionResult> GetActivity(Guid id)
    {
        var activity = await _activityService.GetByIdAsync(id);
        return Ok(activity);
    }

    [HttpPut("/Update/{id}")]
    public async Task<IActionResult> UpdateActivity(Guid id, [FromBody] Activity activity)
    {
        var updatedActivity = await _activityService.UpdateActivityAsync(id, activity);
        return Ok(updatedActivity);
    }

    [HttpDelete("/Delete/{id}")]
    public async Task<IActionResult> DeleteActivity(Guid id)
    {
        await _activityService.DeleteActivityAsync(id);
        return NoContent();
    }
}