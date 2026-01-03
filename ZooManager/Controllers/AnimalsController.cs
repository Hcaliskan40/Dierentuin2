using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ZooManager.Data;
using ZooManager.Models;
using ZooManager.Services;
using ZooManager.ViewModels;

namespace ZooManager.Controllers;

public class AnimalsController : Controller
{
    private readonly ZooDbContext _db;
    private readonly ZooLogicService _logic;

    public AnimalsController(ZooDbContext db, ZooLogicService logic)
    {
        _db = db;
        _logic = logic;
    }

    // FILTER op (bijna) alle eigenschappen via querystring
    // /Animals?name=a&species=lion&size=Large&categoryId=1&enclosureId=2&diet=Carnivore&activity=Diurnal&security=High
    public async Task<IActionResult> Index(
        string? name,
        string? species,
        Size? size,
        DietaryClass? diet,
        ActivityPattern? activity,
        SecurityLevel? security,
        int? categoryId,
        int? enclosureId)
    {
        var q = _db.Animals
            .Include(a => a.Category)
            .Include(a => a.Enclosure)
            .AsQueryable();

        // 🔍 Filters
        if (!string.IsNullOrWhiteSpace(name))
            q = q.Where(a => a.Name.Contains(name));

        if (!string.IsNullOrWhiteSpace(species))
            q = q.Where(a => a.Species.Contains(species));

        if (size.HasValue)
            q = q.Where(a => a.Size == size);

        if (diet.HasValue)
            q = q.Where(a => a.DietaryClass == diet);

        if (activity.HasValue)
            q = q.Where(a => a.ActivityPattern == activity);

        if (security.HasValue)
            q = q.Where(a => a.SecurityRequirement == security);

        if (categoryId.HasValue)
            q = q.Where(a => a.CategoryId == categoryId);

        if (enclosureId.HasValue)
            q = q.Where(a => a.EnclosureId == enclosureId);

        // Data voor dropdowns
        ViewBag.Categories = await _db.Categories.OrderBy(c => c.Name).ToListAsync();
        ViewBag.Enclosures = await _db.Enclosures.OrderBy(e => e.Name).ToListAsync();

        return View(await q.OrderBy(a => a.Name).ToListAsync());
    }


    public async Task<IActionResult> Details(int id)
    {
        var a = await _db.Animals
            .Include(x => x.Category)
            .Include(x => x.Enclosure)
            .Include(x => x.Prey)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (a == null) return NotFound();

        // (optioneel) als jullie dit al tonen in de view, kan je dit houden:
        ViewBag.Sunrise = _logic.AnimalSunrise(a);
        ViewBag.Sunset = _logic.AnimalSunset(a);
        ViewBag.Feeding = await _logic.AnimalFeedingTimeAsync(a.Id);
        ViewBag.Constraints = await _logic.AnimalCheckConstraintsAsync(a.Id);

        return View(a);
    }

    // -------------------------
    // ACTIONS (requirements 2c-2f)
    // -------------------------

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Sunrise(int id)
    {
        var a = await _db.Animals.FirstOrDefaultAsync(x => x.Id == id);
        if (a == null) return NotFound();

        TempData["ActionTitle"] = "Sunrise";
        TempData["ActionResult"] = _logic.AnimalSunrise(a);

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Sunset(int id)
    {
        var a = await _db.Animals.FirstOrDefaultAsync(x => x.Id == id);
        if (a == null) return NotFound();

        TempData["ActionTitle"] = "Sunset";
        TempData["ActionResult"] = _logic.AnimalSunset(a);

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> FeedingTime(int id)
    {
        var a = await _db.Animals.FirstOrDefaultAsync(x => x.Id == id);
        if (a == null) return NotFound();

        TempData["ActionTitle"] = "Feeding time";
        TempData["ActionResult"] = await _logic.AnimalFeedingTimeAsync(id);

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CheckConstraints(int id)
    {
        var a = await _db.Animals.FirstOrDefaultAsync(x => x.Id == id);
        if (a == null) return NotFound();

        var result = await _logic.AnimalCheckConstraintsAsync(id);

        TempData["ActionTitle"] = "CheckConstraints";
        TempData["ActionResult"] =
            $"OK: {result.Ok}\n\nPASSED:\n- {string.Join("\n- ", result.Passed)}\n\nFAILED:\n- {string.Join("\n- ", result.Failed)}";

        return RedirectToAction(nameof(Details), new { id });
    }

    // -------------------------
    // CRUD
    // -------------------------

    public async Task<IActionResult> Create()
    {
        var vm = await BuildAnimalVmAsync(new AnimalEditVm());
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AnimalEditVm vm)
    {
        if (!ModelState.IsValid)
        {
            vm = await BuildAnimalVmAsync(vm);
            return View(vm);
        }

        var a = new Animal
        {
            Name = vm.Name,
            Species = vm.Species,
            Size = vm.Size,
            DietaryClass = vm.DietaryClass,
            ActivityPattern = vm.ActivityPattern,
            SpaceRequirement = vm.SpaceRequirement,
            SecurityRequirement = vm.SecurityRequirement,
            CategoryId = vm.CategoryId,
            EnclosureId = vm.EnclosureId,
            PreyId = vm.PreyId
        };

        _db.Animals.Add(a);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var a = await _db.Animals.FirstOrDefaultAsync(x => x.Id == id);
        if (a == null) return NotFound();

        var vm = new AnimalEditVm
        {
            Id = a.Id,
            Name = a.Name,
            Species = a.Species,
            Size = a.Size,
            DietaryClass = a.DietaryClass,
            ActivityPattern = a.ActivityPattern,
            SpaceRequirement = a.SpaceRequirement,
            SecurityRequirement = a.SecurityRequirement,
            CategoryId = a.CategoryId,
            EnclosureId = a.EnclosureId,
            PreyId = a.PreyId
        };

        vm = await BuildAnimalVmAsync(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AnimalEditVm vm)
    {
        if (id != vm.Id) return BadRequest();

        var a = await _db.Animals.FirstOrDefaultAsync(x => x.Id == id);
        if (a == null) return NotFound();

        if (!ModelState.IsValid)
        {
            vm = await BuildAnimalVmAsync(vm);
            return View(vm);
        }

        a.Name = vm.Name;
        a.Species = vm.Species;
        a.Size = vm.Size;
        a.DietaryClass = vm.DietaryClass;
        a.ActivityPattern = vm.ActivityPattern;
        a.SpaceRequirement = vm.SpaceRequirement;
        a.SecurityRequirement = vm.SecurityRequirement;
        a.CategoryId = vm.CategoryId;
        a.EnclosureId = vm.EnclosureId;
        a.PreyId = vm.PreyId;

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var a = await _db.Animals.FirstOrDefaultAsync(x => x.Id == id);
        if (a == null) return NotFound();
        return View(a);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var a = await _db.Animals.FirstOrDefaultAsync(x => x.Id == id);
        if (a == null) return NotFound();

        _db.Animals.Remove(a);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private async Task<AnimalEditVm> BuildAnimalVmAsync(AnimalEditVm vm)
    {
        var categories = await _db.Categories.OrderBy(c => c.Name).ToListAsync();
        var enclosures = await _db.Enclosures.OrderBy(e => e.Name).ToListAsync();
        var animals = await _db.Animals.OrderBy(a => a.Name).ToListAsync();

        // ✅ FIX: None optie + echte categories
        vm.CategoryOptions = new List<SelectListItem> { new("None", "") };
        vm.CategoryOptions.AddRange(categories.Select(c =>
            new SelectListItem(c.Name, c.Id.ToString())));

        vm.EnclosureOptions = new List<SelectListItem> { new("None", "") };
        vm.EnclosureOptions.AddRange(enclosures.Select(e =>
            new SelectListItem(e.Name, e.Id.ToString())));

        vm.PreyOptions = new List<SelectListItem> { new("None", "") };
        vm.PreyOptions.AddRange(animals.Where(a => a.Id != vm.Id).Select(a =>
            new SelectListItem($"{a.Name} ({a.Species})", a.Id.ToString())));

        return vm;
    }
}
