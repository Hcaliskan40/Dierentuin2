using System.ComponentModel.DataAnnotations;

namespace ZooManager.Models;

public class Animal
{
    public int Id { get; set; }

    [Required, MaxLength(80)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(80)]
    public string Species { get; set; } = string.Empty;

    public AnimalSize Size { get; set; }
    public DietaryClass DietaryClass { get; set; }
    public ActivityPattern ActivityPattern { get; set; }

    [MaxLength(80)]
    public string? Prey { get; set; }

    public double SpaceRequirement { get; set; }
    public SecurityLevel SecurityRequirement { get; set; }

    public int? CategoryId { get; set; }
    public Category? Category { get; set; }

    public int? EnclosureId { get; set; }
    public Enclosure? Enclosure { get; set; }
}