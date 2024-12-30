using System.ComponentModel.DataAnnotations;

namespace Models;

public class BlogPost
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(120)]
    public string Title { get; set; } = string.Empty;

    [Required, MaxLength(125)]
    public string Slug { get; set; } = string.Empty;

    [Required, MaxLength(255)]
    public string Introduction { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    public int CategoryId { get; set; }
    public string? UserId { get; set; }
    public virtual Category? Category { get; set; }
    public bool IsPublished { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? PublishedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
}