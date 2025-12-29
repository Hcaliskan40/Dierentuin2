using System.ComponentModel.DataAnnotations;

namespace ZooManager.Models;

public class Animal
{
    public int Id { get; set; }

    [Required, MaxLength(80)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(80)]
    public string Species { get; set; } = string.Empty;

    // Category: default NULL toegestaan
    public int? CategoryId { get; set; }
    public Category? Category { get; set; }

    public AnimalSize Size { get; set; }

    public DietaryClass DietaryClass { get; set; }

    public ActivityPattern ActivityPattern { get; set; }

    // Prey: veel animals kunnen "prooi" zijn of juist prooien hebben.
    // Simpel gehouden: een Animal kan 0..n prooien hebben.
    public ICollection<Animal> Prey { get; set; } = new List<Animal>();

    // Enclosure: default NULL toegestaan
    public int? EnclosureId { get; set; }
    public Enclosure? Enclosure { get; set; }

    // in square meters per animal
    [Range(0.1, double.MaxValue)]
    public double SpaceRequirement { get; set; }

    public SecurityLevel SecurityRequirement { get; set; }
}