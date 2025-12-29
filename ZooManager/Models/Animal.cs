using System.ComponentModel.DataAnnotations;

namespace ZooManager.Models;

public class Animal
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Species { get; set; } = string.Empty;

    public Size Size { get; set; }

    public DietaryClass DietaryClass { get; set; }

    public ActivityPattern ActivityPattern { get; set; }

    // Nullable volgens opdracht (default NULL mogelijk)
    public int? CategoryId { get; set; }
    public Category? Category { get; set; }

    // Nullable volgens opdracht (default NULL mogelijk)
    public int? EnclosureId { get; set; }
    public Enclosure? Enclosure { get; set; }

    // Self-reference (prooi)
    public int? PreyId { get; set; }
    public Animal? Prey { get; set; }

    // m2 per animal
    public double SpaceRequirement { get; set; }

    public SecurityLevel SecurityRequirement { get; set; }
}