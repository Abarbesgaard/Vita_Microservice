using System.Text.Json.Serialization;

namespace Vitahus_VideoService_Shared;

public class Video : BaseEntity
{
    [JsonPropertyName("url")]
    public required string? Url { get; set; }

    [JsonPropertyName("Visible")]
    public bool Visible { get; set; } = false;

    [JsonPropertyName("GroupId")]
    public Guid? GroupId { get; set; }

    [JsonPropertyName("GroupNumber")]
    public int? GroupNumber { get; set; }
}
