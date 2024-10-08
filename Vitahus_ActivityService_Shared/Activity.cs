using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Vitahus_ActivityService_Shared;

public class Activity : BaseEntity
{
    [JsonPropertyName("Time_start")]
    [DataType(DataType.DateTime)]
    public DateTimeOffset Start { get; set; }

    [JsonPropertyName("Time_end")]
    [DataType(DataType.DateTime)]
    public DateTimeOffset End { get; set; }
    
    [JsonPropertyName("Cancelled")] public bool Cancelled { get; set; } = false;

    [JsonPropertyName("AllDayEvent")] public bool AllDayEvent { get; set; } = false;
}