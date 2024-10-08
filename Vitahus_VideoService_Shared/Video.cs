using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Vitahus_VideoService_Shared;

public class Video : BaseEntity
{
    [JsonPropertyName("Url")]
    [StringLength(
        2083,
        MinimumLength = 1,
        ErrorMessage = "Url must be between 1 and 2083 characters"
    )]
    public required string? Url { get; set; }

    [JsonPropertyName("Visible")]
    public bool Visible { get; set; } = false;

    [JsonPropertyName("GroupId")]
    public Guid? GroupId { get; set; }

    [JsonPropertyName("GroupNumber")]
    public int? GroupNumber { get; set; }
}
