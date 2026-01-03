using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ZooManager.Data;
using ZooManager.Models;

namespace ZooManager.Controllers;

public class AnimalsController : Controller
{
    private readonly ZooDbContext _db;

    public AnimalsController(ZooDbContext db)
    {
        _db = db;
    }

    // GET: /Animals
    public async Task<IActionResult> Index()
    {
        var animals = await _db.Animals
            .Include(a => a.Category)
            .Include(a => a.Enclosure)
            .ToListAsync();

        return View(animals);
    }

    // GET: /Animals/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var animal = await _db.Animals
            .Include(a => a.Category)
            .Include(a => a.Enclosure)
            .Include(a => a.Prey)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (animal == null) return NotFound();
        return View(animal);
    }

    // GET: /Animals/Create
    public async Task<IActionResult> Create()
    {
        await FillDropdowns();
        return View();
    }

    // POST: /Animals/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Animal animal)
    {
        if (!ModelState.IsValid)
        {
            await FillDropdowns();
            return View(animal);
        }

        _db.Animals.Add(animal);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: /Animals/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var animal = await _db.Animals.FindAsync(id);
        if (animal == null) return NotFound();

        await FillDropdowns(animal.CategoryId, animal.EnclosureId, animal.PreyId);
        return View(animal);
    }

    // POST: /Animals/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Animal animal)
    {
        if (id != animal.Id) return NotFound();

        if (!ModelState.IsValid)
        {
            await FillDropdowns(animal.CategoryId, animal.EnclosureId, animal.PreyId);
            return View(animal);
        }

        _db.Update(animal);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: /Animals/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var animal = await _db.Animals
            .Include(a => a.Category)
            .Include(a => a.Enclosure)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (animal == null) return NotFound();
        return View(animal);
    }

    // POST: /Animals/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var animal = await _db.Animals.FindAsync(id);
        if (animal != null)
        {
            _db.Animals.Remove(animal);
            await _db.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task FillDropdowns(int? selectedCategoryId = null, int? selectedEnclosureId = null, int? selectedPreyId = null)
    {
        var categories = await _db.Categories.OrderBy(c => c.Name).ToListAsync();
        var enclosures = await _db.Enclosures.OrderBy(e => e.Name).ToListAsync();
        var animals = await _db.Animals.OrderBy(a => a.Name).ToListAsync();

        ViewBag.CategoryId = new SelectList(categories, "Id", "Name", selectedCategoryId);
        ViewBag.EnclosureId = new SelectList(enclosures, "Id", "Name", selectedEnclosureId);
        ViewBag.PreyId = new SelectList(animals, "Id", "Name", selectedPreyId);
    }
}
