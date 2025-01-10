using System.Text.Json.Serialization;

namespace Models;

public class BlogCategory
{
    [JsonPropertyName("BlogId")]
    public int BlogId { get; set; }

    [JsonPropertyName("Title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("Introduction")]
    public string Introduction { get; set; } = string.Empty;

    [JsonPropertyName("Name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("IsPublished")]
    public bool IsPublished { get; set; }

    [JsonPropertyName("CreatedOn")]
    public DateTime CreatedOn { get; set; }

    [JsonPropertyName("PublishedOn")]
    public DateTime? PublishedOn { get; set; }

    [JsonPropertyName("ModifiedOn")]
    public DateTime? ModifiedOn { get; set; }
}