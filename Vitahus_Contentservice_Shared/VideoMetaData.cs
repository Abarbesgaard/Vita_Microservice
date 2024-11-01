namespace Vitahus_Contentservice_Shared;

public class VideoMetadata
{
  [JsonPropertyName("id")]
  public Guid Id { get; set; }
  [JsonPropertyName("title")]
  public string Title { get; set; }
  [JsonPropertyName("description")]
  public string Description { get; set; }
  [JsonPropertyName("tags")]
  public List<string> Tags { get; set; }
  [JsonPropertyName("category")]
  public string Category { get; set; }
  [JsonPropertyName("uploadDate")]
  public DateTime UploadDate { get; set; }
  [JsonPropertyName("uploaderId")]
  public Guid UploaderId { get; set; }
  [JsonPropertyName("uploaderName")]
  public string Status { get; set; }

}
