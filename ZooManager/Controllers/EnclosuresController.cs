using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZooManager.Data;
using ZooManager.Models;

namespace ZooManager.Controllers;

public class EnclosuresController : Controller
{
    private readonly ZooDbContext _db;

    public EnclosuresController(ZooDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _db.Enclosures.OrderBy(e => e.Name).ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var enclosure = await _db.Enclosures
            .Include(e => e.Animals)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (enclosure == null) return NotFound();

        return View(enclosure);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Enclosure enclosure)
    {
        if (!ModelState.IsValid) return View(enclosure);

        _db.Enclosures.Add(enclosure);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var enclosure = await _db.Enclosures.FindAsync(id);
        if (enclosure == null) return NotFound();

        return View(enclosure);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Enclosure enclosure)
    {
        if (id != enclosure.Id) return NotFound();
        if (!ModelState.IsValid) return View(enclosure);

        _db.Update(enclosure);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var enclosure = await _db.Enclosures.FirstOrDefaultAsync(e => e.Id == id);
        if (enclosure == null) return NotFound();

        return View(enclosure);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var enclosure = await _db.Enclosures.FindAsync(id);
        if (enclosure == null) return RedirectToAction(nameof(Index));

        _db.Enclosures.Remove(enclosure);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
