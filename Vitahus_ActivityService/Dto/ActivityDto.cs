namespace Vitahus_ActivityService.Dto;

public class ActivityDto
{
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
    public bool Cancelled { get; set; } = false;
    public bool AllDayEvent { get; set; } = false;
}