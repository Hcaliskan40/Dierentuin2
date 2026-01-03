using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

    // GET: /Enclosures
    public async Task<IActionResult> Index()
    {
        var enclosures = await _db.Enclosures
            .AsNoTracking()
            .OrderBy(e => e.Name)
            .ToListAsync();

        return View(enclosures);
    }

    // GET: /Enclosures/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var enclosure = await _db.Enclosures
            .AsNoTracking()
            .Include(e => e.Animals)
            .FirstOrDefaultAsync(e => e.Id == id.Value);

        if (enclosure == null) return NotFound();

        return View(enclosure);
    }

    // GET: /Enclosures/Create
    public IActionResult Create()
    {
        PopulateEnums();
        return View(new Enclosure());
    }

    // POST: /Enclosures/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Enclosure enclosure, string[] selectedHabitats)
    {
        enclosure.HabitatType = ParseHabitatFlags(selectedHabitats);

        if (!ModelState.IsValid)
        {
            PopulateEnums(enclosure);
            return View(enclosure);
        }

        _db.Enclosures.Add(enclosure);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // GET: /Enclosures/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var enclosure = await _db.Enclosures.FindAsync(id.Value);
        if (enclosure == null) return NotFound();

        PopulateEnums(enclosure);
        return View(enclosure);
    }

    // POST: /Enclosures/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Enclosure enclosure, string[] selectedHabitats)
    {
        if (id != enclosure.Id) return NotFound();

        enclosure.HabitatType = ParseHabitatFlags(selectedHabitats);

        if (!ModelState.IsValid)
        {
            PopulateEnums(enclosure);
            return View(enclosure);
        }

        _db.Entry(enclosure).State = EntityState.Modified;
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // GET: /Enclosures/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var enclosure = await _db.Enclosures
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id.Value);

        if (enclosure == null) return NotFound();

        return View(enclosure);
    }

    // POST: /Enclosures/Delete/5
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

    private void PopulateEnums(Enclosure? enclosure = null)
    {
        ViewBag.Climates = new SelectList(Enum.GetValues(typeof(Climate)).Cast<Climate>(), enclosure?.Climate);
        ViewBag.SecurityLevels = new SelectList(Enum.GetValues(typeof(SecurityLevel)).Cast<SecurityLevel>(), enclosure?.SecurityLevel);
    }

    private static HabitatType ParseHabitatFlags(string[] selectedHabitats)
    {
        HabitatType result = 0;

        foreach (var s in selectedHabitats ?? Array.Empty<string>())
        {
            if (Enum.TryParse<HabitatType>(s, out var value))
            {
                result |= value;
            }
        }

        return result;
    }
}
