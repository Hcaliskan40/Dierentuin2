using System.ComponentModel.DataAnnotations;

namespace ZooManager.Models;

public class Enclosure
{
    public int Id { get; set; }

    [Required, MaxLength(80)]
    public string Name { get; set; } = string.Empty;

    public Climate Climate { get; set; }

    public HabitatType HabitatType { get; set; }

    public SecurityLevel SecurityLevel { get; set; }

    // in square meters
    [Range(1, double.MaxValue)]
    public double Size { get; set; }

    // One-to-many
    public ICollection<Animal> Animals { get; set; } = new List<Animal>();
}