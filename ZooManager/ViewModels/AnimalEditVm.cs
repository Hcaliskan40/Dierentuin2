using Microsoft.AspNetCore.Mvc.Rendering;
using ZooManager.Models;

namespace ZooManager.ViewModels;

public class AnimalEditVm
{
    public int Id { get; set; }

    public string Name { get; set; } = "";
    public string Species { get; set; } = "";

    public Size Size { get; set; }
    public DietaryClass DietaryClass { get; set; }
    public ActivityPattern ActivityPattern { get; set; }

    public double SpaceRequirement { get; set; }
    public SecurityLevel SecurityRequirement { get; set; }

    public int? CategoryId { get; set; }
    public int? EnclosureId { get; set; }
    public int? PreyId { get; set; }

    public List<SelectListItem> CategoryOptions { get; set; } = new();
    public List<SelectListItem> EnclosureOptions { get; set; } = new();
    public List<SelectListItem> PreyOptions { get; set; } = new();
}