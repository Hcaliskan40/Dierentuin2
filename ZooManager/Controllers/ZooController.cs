using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZooManager.Data;
using ZooManager.Models;

namespace ZooManager.Controllers;

public class ZooController : Controller
{
    private readonly ZooDbContext _db;

    public ZooController(ZooDbContext db)
    {
        _db = db;
    }

    // Dashboard
    public async Task<IActionResult> Index()
    {
        ViewBag.AnimalCount = await _db.Animals.CountAsync();
        ViewBag.CategoryCount = await _db.Categories.CountAsync();
        ViewBag.EnclosureCount = await _db.Enclosures.CountAsync();

        var unassigned = await _db.Animals.CountAsync(a => a.EnclosureId == null);
        ViewBag.UnassignedCount = unassigned;

        return View();
    }

    // 5a - Zoo Sunrise
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Sunrise()
    {
        var animals = await LoadAnimalsAsync();
        var lines = animals
            .OrderBy(a => a.Name)
            .Select(a => $"{a.Name} ({a.Species}): {ZooSunriseText(a.ActivityPattern)}")
            .ToList();

        TempData["ZooActionTitle"] = "Zoo - Sunrise";
        TempData["ZooActionResult"] = string.Join("\n", lines);
        return RedirectToAction(nameof(Index));
    }

    // 5b - Zoo Sunset
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Sunset()
    {
        var animals = await LoadAnimalsAsync();
        var lines = animals
            .OrderBy(a => a.Name)
            .Select(a => $"{a.Name} ({a.Species}): {ZooSunsetText(a.ActivityPattern)}")
            .ToList();

        TempData["ZooActionTitle"] = "Zoo - Sunset";
        TempData["ZooActionResult"] = string.Join("\n", lines);
        return RedirectToAction(nameof(Index));
    }

    // 5c - Zoo Feeding time
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> FeedingTime()
    {
        var animals = await LoadAnimalsAsync();

        // per enclosure groeperen (incl. unassigned)
        var groups = animals
            .GroupBy(a => a.Enclosure?.Name ?? "Unassigned")
            .OrderBy(g => g.Key);

        var output = new List<string>();

        foreach (var g in groups)
        {
            output.Add($"== {g.Key} ==");
            var list = g.OrderBy(a => a.Name).ToList();

            foreach (var a in list)
            {
                var food = DetermineFood(a, list);
                output.Add($"- {a.Name} ({a.Species}): {food}");
            }

            output.Add("");
        }

        TempData["ZooActionTitle"] = "Zoo - Feeding time";
        TempData["ZooActionResult"] = string.Join("\n", output);
        return RedirectToAction(nameof(Index));
    }

    // 5d - Zoo CheckConstraints
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CheckConstraints()
    {
        var enclosures = await _db.Enclosures
            .Include(e => e.Animals)
                .ThenInclude(a => a.Prey)
            .OrderBy(e => e.Name)
            .ToListAsync();

        var output = new List<string>();
        var okAll = true;

        foreach (var e in enclosures)
        {
            var animals = e.Animals?.ToList() ?? new List<Animal>();

            var requiredSpace = animals.Sum(a => a.SpaceRequirement);
            var spaceOk = requiredSpace <= e.Size;

            var maxSecNeeded = animals.Any() ? animals.Max(a => (int)a.SecurityRequirement) : 0;
            var secOk = (int)e.SecurityLevel >= maxSecNeeded;

            // predation conflict: als predator zijn prey in hetzelfde verblijf heeft
            var predationConflicts = animals
                .Where(a => a.PreyId != null && animals.Any(x => x.Id == a.PreyId))
                .Select(a =>
                {
                    var prey = animals.First(x => x.Id == a.PreyId);
                    return $"{a.Name} may eat {prey.Name}";
                })
                .ToList();

            var enclosureOk = spaceOk && secOk && predationConflicts.Count == 0;
            okAll &= enclosureOk;

            output.Add($"== {e.Name} ==");
            output.Add($"Space: {requiredSpace:0.##} / {e.Size:0.##} m2 -> {(spaceOk ? "OK" : "FAIL")}");
            output.Add($"Security: required {((SecurityLevel)maxSecNeeded)} / enclosure {e.SecurityLevel} -> {(secOk ? "OK" : "FAIL")}");

            if (predationConflicts.Count > 0)
            {
                output.Add("Predation conflicts -> FAIL");
                foreach (var c in predationConflicts)
                    output.Add($"- {c}");
            }
            else
            {
                output.Add("Predation conflicts -> OK");
            }

            output.Add($"Result: {(enclosureOk ? "OK" : "FAIL")}");
            output.Add("");
        }

        TempData["ZooActionTitle"] = "Zoo - CheckConstraints";
        TempData["ZooActionResult"] = (okAll ? "OK ✅\n\n" : "NOT OK ❌\n\n") + string.Join("\n", output);

        return RedirectToAction(nameof(Index));
    }

    // 5e - Zoo AutoAssign
    // mode = "finish"  -> bestaande verblijven behouden en aanvullen, extra verblijven maken als nodig
    // mode = "reset"   -> alle verblijven verwijderen + nieuwe indeling maken
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AutoAssign(string mode)
    {
        var finishExisting = mode.Equals("finish", StringComparison.OrdinalIgnoreCase);

        if (!finishExisting)
        {
            // reset: animals loskoppelen en verblijven verwijderen
            var animals = await _db.Animals.ToListAsync();
            foreach (var a in animals)
                a.EnclosureId = null;

            var enclosures = await _db.Enclosures.ToListAsync();
            _db.Enclosures.RemoveRange(enclosures);

            await _db.SaveChangesAsync();
        }

        // opnieuw laden
        var allAnimals = await _db.Animals.Include(a => a.Enclosure).ToListAsync();
        var existingEnclosures = await _db.Enclosures.Include(e => e.Animals).ToListAsync();

        // we assignen alleen unassigned dieren
        var unassigned = allAnimals
            .Where(a => a.EnclosureId == null)
            .OrderByDescending(a => (int)a.SecurityRequirement)
            .ThenByDescending(a => a.SpaceRequirement)
            .ToList();

        var created = 0;
        var assigned = 0;

        foreach (var animal in unassigned)
        {
            // probeer bestaande verblijf (als mode=finish)
            var placed = false;

            foreach (var e in existingEnclosures.OrderBy(e => e.Name))
            {
                if (CanPlaceInEnclosure(animal, e))
                {
                    animal.EnclosureId = e.Id;
                    assigned++;
                    placed = true;
                    break;
                }
            }

            if (placed) continue;

            // nieuw verblijf maken
            var newEnc = new Enclosure
            {
                Name = $"Auto Enclosure {existingEnclosures.Count + 1}",
                Climate = Climate.Temperate,
                HabitatType = HabitatType.Grassland,
                SecurityLevel = (SecurityLevel)Math.Max((int)SecurityLevel.Low, (int)animal.SecurityRequirement),
                Size = Math.Max(150, animal.SpaceRequirement * 10) // simpele default
            };

            _db.Enclosures.Add(newEnc);
            await _db.SaveChangesAsync();

            existingEnclosures.Add(newEnc);
            created++;

            animal.EnclosureId = newEnc.Id;
            assigned++;
        }

        await _db.SaveChangesAsync();

        TempData["ZooActionTitle"] = "Zoo - AutoAssign";
        TempData["ZooActionResult"] =
            $"Mode: {(finishExisting ? "Finish existing" : "Reset + new")}\n" +
            $"Assigned animals: {assigned}\n" +
            $"New enclosures created: {created}";

        return RedirectToAction(nameof(Index));
    }

    // --------------------
    // Helpers
    // --------------------

    private async Task<List<Animal>> LoadAnimalsAsync()
    {
        return await _db.Animals
            .Include(a => a.Category)
            .Include(a => a.Enclosure)
            .Include(a => a.Prey)
            .ToListAsync();
    }

    private static string ZooSunriseText(ActivityPattern p) =>
        p switch
        {
            ActivityPattern.Diurnal => "wakes up",
            ActivityPattern.Nocturnal => "goes to sleep",
            ActivityPattern.Cathemeral => "stays active",
            _ => "no change"
        };

    private static string ZooSunsetText(ActivityPattern p) =>
        p switch
        {
            ActivityPattern.Diurnal => "goes to sleep",
            ActivityPattern.Nocturnal => "wakes up",
            ActivityPattern.Cathemeral => "stays active",
            _ => "no change"
        };

    private static string DetermineFood(Animal a, List<Animal> sameEnclosureAnimals)
    {
        // Prey (als prey in hetzelfde verblijf zit, gaat dat boven gegeven eten)
        if (a.PreyId != null)
        {
            var prey = sameEnclosureAnimals.FirstOrDefault(x => x.Id == a.PreyId);
            if (prey != null)
                return $"eats {prey.Name} ({prey.Species})";
        }

        return a.DietaryClass switch
        {
            DietaryClass.Carnivore => "eats meat",
            DietaryClass.Herbivore => "eats plants",
            DietaryClass.Omnivore => "eats plants + meat",
            DietaryClass.Insectivore => "eats insects",
            DietaryClass.Piscivore => "eats fish",
            _ => "eats food"
        };
    }

    private static bool CanPlaceInEnclosure(Animal a, Enclosure e)
    {
        // security check
        if ((int)e.SecurityLevel < (int)a.SecurityRequirement)
            return false;

        var animals = e.Animals?.ToList() ?? new List<Animal>();

        // space check (simpel)
        var required = animals.Sum(x => x.SpaceRequirement) + a.SpaceRequirement;
        if (required > e.Size)
            return false;

        // predation conflict: als a prey eet die al in verblijf zit of andersom
        if (a.PreyId != null && animals.Any(x => x.Id == a.PreyId))
            return false;

        if (animals.Any(x => x.PreyId == a.Id))
            return false;

        return true;
    }
}
