using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ZooManager.Data;
using ZooManager.Models;
using ZooManager.ViewModels;

namespace ZooManager.Controllers;

public class EnclosuresController : Controller
{
    private readonly ZooDbContext _db;

    public EnclosuresController(ZooDbContext db)
    {
        _db = db;
    }

    // GET: /Enclosures
    public async Task<IActionResult> Index(string? name)
    {
        var q = _db.Enclosures
            .Include(e => e.Animals)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
            q = q.Where(e => e.Name.Contains(name));

        ViewBag.Name = name;
        return View(await q.OrderBy(e => e.Name).ToListAsync());
    }

    // GET: /Enclosures/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var e = await _db.Enclosures
            .Include(x => x.Animals)
                .ThenInclude(a => a.Category)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (e == null) return NotFound();
        return View(e);
    }

    // GET: /Enclosures/Create
    public IActionResult Create()
    {
        return View(new EnclosureEditVm());
    }

    // POST: /Enclosures/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EnclosureEditVm vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var e = new Enclosure
        {
            Name = vm.Name,
            Climate = vm.Climate,
            HabitatType = vm.HabitatType,
            SecurityLevel = vm.SecurityLevel,
            Size = vm.Size
        };

        _db.Enclosures.Add(e);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: /Enclosures/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var e = await _db.Enclosures.FirstOrDefaultAsync(x => x.Id == id);
        if (e == null) return NotFound();

        var vm = new EnclosureEditVm
        {
            Id = e.Id,
            Name = e.Name,
            Climate = e.Climate,
            HabitatType = e.HabitatType,
            SecurityLevel = e.SecurityLevel,
            Size = e.Size
        };

        return View(vm);
    }

    // POST: /Enclosures/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EnclosureEditVm vm)
    {
        if (vm.Id == null || id != vm.Id) return BadRequest();
        if (!ModelState.IsValid) return View(vm);

        var e = await _db.Enclosures.FirstOrDefaultAsync(x => x.Id == id);
        if (e == null) return NotFound();

        e.Name = vm.Name;
        e.Climate = vm.Climate;
        e.HabitatType = vm.HabitatType;
        e.SecurityLevel = vm.SecurityLevel;
        e.Size = vm.Size;

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: /Enclosures/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var e = await _db.Enclosures.FirstOrDefaultAsync(x => x.Id == id);
        if (e == null) return NotFound();
        return View(e);
    }

    // POST: /Enclosures/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var e = await _db.Enclosures
            .Include(x => x.Animals)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (e == null) return NotFound();

        // dieren erin -> EnclosureId = NULL (vereiste: default NULL optie)
        foreach (var a in e.Animals)
            a.EnclosureId = null;

        _db.Enclosures.Remove(e);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
