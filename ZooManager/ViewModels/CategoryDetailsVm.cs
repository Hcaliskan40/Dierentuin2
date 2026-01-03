using Microsoft.AspNetCore.Mvc.Rendering;
using ZooManager.Models;

namespace ZooManager.ViewModels;

public class CategoryDetailsVm
{
    public Category Category { get; set; } = default!;
    public List<Animal> AnimalsInCategory { get; set; } = new();
    public List<SelectListItem> AssignableAnimals { get; set; } = new();

    public int? SelectedAnimalId { get; set; }
}