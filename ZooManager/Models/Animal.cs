using System.ComponentModel.DataAnnotations;

namespace ZooManager.Models;

public class Animal
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Species { get; set; } = string.Empty;

    public Size Size { get; set; }
    public DietaryClass DietaryClass { get; set; }
    public ActivityPattern ActivityPattern { get; set; }

    // ✅ Category (mag NULL zijn -> "None")
    public int? CategoryId { get; set; }
    public Category? Category { get; set; }

    // ✅ Enclosure (mag NULL zijn)
    public int? EnclosureId { get; set; }
    public Enclosure? Enclosure { get; set; }

    // ✅ Prey (mag NULL zijn)
    public int? PreyId { get; set; }
    public Animal? Prey { get; set; }

    public double SpaceRequirement { get; set; }
    public SecurityLevel SecurityRequirement { get; set; }
}