using System.ComponentModel.DataAnnotations;

namespace Models;

public class Category
{
    [Key]
    public int Id { get; set; }

    [Required, StringLength(maximumLength: 125, MinimumLength = 4, ErrorMessage = "Name must be between 4 and 125 characters")]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(125)]
    public string Slug { get; set; } = string.Empty;
}