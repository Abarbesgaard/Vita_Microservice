using Microsoft.AspNetCore.Mvc;
using Vitahus_ActivityService_Service;
using Vitahus_ActivityService_Shared;

namespace Vitahus_ActivityService.Controller;

public class ActivityController(
    IActivityService activityService, 
    ILogger<ActivityController> logger) 
: ControllerBase
{
    private readonly IActivityService _activityService = activityService;

    [HttpPost("/Create")]
    public async Task<IActionResult> CreateActivity([FromBody] Activity activity)
    {
        logger.LogInformation("Creating activity");
        var createdActivity = await _activityService.CreateActivityAsync(activity);
        return CreatedAtAction(nameof(GetActivity), new { id = createdActivity.Id }, createdActivity);
    }

    [HttpGet("/GetAll")]
    public async Task<IActionResult> GetAllActivities()
    {
        logger.LogInformation("Getting all activities");
        var activities = await _activityService.GetAllAsync();
        return Ok(activities);
    }

    [HttpGet("/GetById/{id}")]
    public async Task<IActionResult> GetActivity(Guid id)
    {
        logger.LogInformation("Getting activity by ID: {Id}", id);
        var activity = await _activityService.GetByIdAsync(id);
        return Ok(activity);
    }

    [HttpPut("/Update/{id}")]
    public async Task<IActionResult> UpdateActivity(Guid id, [FromBody] Activity activity)
    {
        logger.LogInformation("Updating activity with ID: {Id}", id);
        var updatedActivity = await _activityService.UpdateActivityAsync(id, activity);
        return Ok(updatedActivity);
    }

    [HttpDelete("/Delete/{id}")]
    public async Task<IActionResult> DeleteActivity(Guid id)
    {
        logger.LogInformation("Deleting activity with ID: {Id}", id);
        await _activityService.DeleteActivityAsync(id);
        return NoContent();
    }
}