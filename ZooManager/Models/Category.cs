using System.ComponentModel.DataAnnotations;

namespace ZooManager.Models;

public class Category
{
    public int Id { get; set; }

    [Required, MaxLength(80)]
    public string Name { get; set; } = string.Empty;

    // Many-to-many
    public ICollection<Animal> Animals { get; set; } = new List<Animal>();
}