using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ZooManager.Data;
using ZooManager.Models;
using ZooManager.ViewModels;

namespace ZooManager.Controllers;

public class CategoriesController : Controller
{
    private readonly ZooDbContext _db;

    public CategoriesController(ZooDbContext db)
    {
        _db = db;
    }

    // 3d: zoeken/filteren op categorie
    // /Categories?search=birds
    public async Task<IActionResult> Index(string? search)
    {
        var q = _db.Categories.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            q = q.Where(c => c.Name.Contains(search));

        var list = await q.OrderBy(c => c.Name).ToListAsync();
        ViewBag.Search = search;

        return View(list);
    }

    public async Task<IActionResult> Details(int id)
    {
        var category = await _db.Categories.FirstOrDefaultAsync(c => c.Id == id);
        if (category == null) return NotFound();

        var animalsInCategory = await _db.Animals
            .Where(a => a.CategoryId == id)
            .OrderBy(a => a.Name)
            .ToListAsync();

        // Alleen dieren die nog geen categorie hebben (default NULL)
        var unassignedAnimals = await _db.Animals
            .Where(a => a.CategoryId == null)
            .OrderBy(a => a.Name)
            .ToListAsync();

        var vm = new CategoryDetailsVm
        {
            Category = category,
            AnimalsInCategory = animalsInCategory,
            AssignableAnimals = new List<SelectListItem>
            {
                new("Choose animal...", "")
            }
        };

        vm.AssignableAnimals.AddRange(unassignedAnimals.Select(a =>
            new SelectListItem($"{a.Name} ({a.Species})", a.Id.ToString())
        ));

        return View(vm);
    }

    public IActionResult Create()
    {
        return View(new Category());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Category category)
    {
        if (!ModelState.IsValid) return View(category);

        _db.Categories.Add(category);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var category = await _db.Categories.FirstOrDefaultAsync(c => c.Id == id);
        if (category == null) return NotFound();
        return View(category);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Category category)
    {
        if (id != category.Id) return BadRequest();
        if (!ModelState.IsValid) return View(category);

        var dbCat = await _db.Categories.FirstOrDefaultAsync(c => c.Id == id);
        if (dbCat == null) return NotFound();

        dbCat.Name = category.Name;
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var category = await _db.Categories.FirstOrDefaultAsync(c => c.Id == id);
        if (category == null) return NotFound();
        return View(category);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var category = await _db.Categories.FirstOrDefaultAsync(c => c.Id == id);
        if (category == null) return NotFound();

        // dieren die deze categorie hebben -> terug naar NULL (default)
        var animals = await _db.Animals.Where(a => a.CategoryId == id).ToListAsync();
        foreach (var a in animals) a.CategoryId = null;

        _db.Categories.Remove(category);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // 3c: dier toekennen aan categorie
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignAnimal(int categoryId, int? selectedAnimalId)
    {
        if (selectedAnimalId == null)
            return RedirectToAction(nameof(Details), new { id = categoryId });

        var animal = await _db.Animals.FirstOrDefaultAsync(a => a.Id == selectedAnimalId.Value);
        if (animal == null) return NotFound();

        animal.CategoryId = categoryId;
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = categoryId });
    }

    // 3c: dier losmaken (Category -> NULL)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UnassignAnimal(int categoryId, int animalId)
    {
        var animal = await _db.Animals.FirstOrDefaultAsync(a => a.Id == animalId);
        if (animal == null) return NotFound();

        if (animal.CategoryId == categoryId)
        {
            animal.CategoryId = null;
            await _db.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Details), new { id = categoryId });
    }
}
