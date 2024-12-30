using System.ComponentModel.DataAnnotations;

namespace Models;

public class Category
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(125)]
    public string Slug { get; set; } = string.Empty;
}