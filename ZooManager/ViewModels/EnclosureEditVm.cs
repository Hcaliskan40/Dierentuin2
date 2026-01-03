using System.ComponentModel.DataAnnotations;
using ZooManager.Models;

namespace ZooManager.ViewModels;

public class EnclosureEditVm
{
    public int? Id { get; set; }

    [Required]
    [StringLength(120)]
    public string Name { get; set; } = "";

    public Climate Climate { get; set; }
    public HabitatType HabitatType { get; set; }
    public SecurityLevel SecurityLevel { get; set; }

    [Range(1, 100000)]
    public double Size { get; set; }
}