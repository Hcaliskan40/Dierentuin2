using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZooManager.Data;
using ZooManager.Models;

namespace ZooManager.Controllers;

public class CategoriesController : Controller
{
    private readonly ZooDbContext _db;

    public CategoriesController(ZooDbContext db)
    {
        _db = db;
    }

    // GET: /Categories
    public async Task<IActionResult> Index()
    {
        var categories = await _db.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync();

        return View(categories);
    }

    // GET: /Categories/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var category = await _db.Categories
            .AsNoTracking()
            .Include(c => c.Animals)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null) return NotFound();

        return View(category);
    }

    // GET: /Categories/Create
    public IActionResult Create()
    {
        return View(new Category());
    }

    // POST: /Categories/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Category category)
    {
        if (!ModelState.IsValid) return View(category);

        _db.Categories.Add(category);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // GET: /Categories/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var category = await _db.Categories.FindAsync(id.Value);
        if (category == null) return NotFound();

        return View(category);
    }

    // POST: /Categories/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Category category)
    {
        if (id != category.Id) return NotFound();
        if (!ModelState.IsValid) return View(category);

        _db.Entry(category).State = EntityState.Modified;
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // GET: /Categories/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var category = await _db.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id.Value);

        if (category == null) return NotFound();

        return View(category);
    }

    // POST: /Categories/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var category = await _db.Categories.FindAsync(id);
        if (category == null) return RedirectToAction(nameof(Index));

        _db.Categories.Remove(category);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
