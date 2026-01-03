using Microsoft.EntityFrameworkCore;
using ZooManager.Data;
using ZooManager.Models;

namespace ZooManager.Services;

public record ActionResultLine(int Id, string Name, string Message);

public record ConstraintResult(
    bool Ok,
    List<string> Passed,
    List<string> Failed
);

public class ZooLogicService
{
    private readonly ZooDbContext _db;

    public ZooLogicService(ZooDbContext db)
    {
        _db = db;
    }

    // ----------------------
    // Animal actions
    // ----------------------

    public string AnimalSunrise(Animal a)
    {
        return a.ActivityPattern switch
        {
            ActivityPattern.Diurnal => "Wakes up (diurnal).",
            ActivityPattern.Nocturnal => "Goes to sleep (nocturnal).",
            ActivityPattern.Cathemeral => "Always active (cathemeral).",
            _ => "Unknown pattern."
        };
    }

    public string AnimalSunset(Animal a)
    {
        return a.ActivityPattern switch
        {
            ActivityPattern.Diurnal => "Goes to sleep (diurnal).",
            ActivityPattern.Nocturnal => "Wakes up (nocturnal).",
            ActivityPattern.Cathemeral => "Always active (cathemeral).",
            _ => "Unknown pattern."
        };
    }

    public async Task<string> AnimalFeedingTimeAsync(int animalId)
    {
        var a = await _db.Animals
            .Include(x => x.Prey)
            .Include(x => x.Enclosure)
            .FirstOrDefaultAsync(x => x.Id == animalId);

        if (a == null) return "Animal not found.";

        // If prey is set and prey exists and is in same enclosure => eats prey
        if (a.PreyId != null && a.Prey != null)
        {
            if (a.EnclosureId != null && a.Prey.EnclosureId == a.EnclosureId)
                return $"Eats prey: {a.Prey.Name} ({a.Prey.Species}).";
        }

        // Otherwise eats based on dietary class
        return a.DietaryClass switch
        {
            DietaryClass.Carnivore => "Eats meat.",
            DietaryClass.Herbivore => "Eats plants.",
            DietaryClass.Omnivore => "Eats plants and meat.",
            DietaryClass.Insectivore => "Eats insects.",
            DietaryClass.Piscivore => "Eats fish.",
            _ => "Unknown diet."
        };
    }

    public async Task<ConstraintResult> AnimalCheckConstraintsAsync(int animalId)
    {
        var a = await _db.Animals
            .Include(x => x.Enclosure)
            .FirstOrDefaultAsync(x => x.Id == animalId);

        if (a == null)
        {
            return new ConstraintResult(false, new(), new() { "Animal not found." });
        }

        var passed = new List<string>();
        var failed = new List<string>();

        // Category/Enclosure may be null (allowed by assignment)
        passed.Add("Category can be NULL (allowed).");
        passed.Add("Enclosure can be NULL (allowed).");

        if (a.Enclosure == null)
        {
            failed.Add("No enclosure assigned (allowed, but constraints cannot be fully validated).");
            return new ConstraintResult(false, passed, failed);
        }

        // Security check
        if (a.Enclosure.SecurityLevel >= a.SecurityRequirement)
            passed.Add($"Security OK: enclosure {a.Enclosure.SecurityLevel} >= animal {a.SecurityRequirement}");
        else
            failed.Add($"Security FAIL: enclosure {a.Enclosure.SecurityLevel} < animal {a.SecurityRequirement}");

        // Space check: total space requirements in enclosure
        var animalsInEnclosure = await _db.Animals
            .Where(x => x.EnclosureId == a.EnclosureId)
            .ToListAsync();

        var used = animalsInEnclosure.Sum(x => x.SpaceRequirement);
        if (used <= a.Enclosure.Size)
            passed.Add($"Space OK: used {used:0.##} <= enclosure size {a.Enclosure.Size:0.##}");
        else
            failed.Add($"Space FAIL: used {used:0.##} > enclosure size {a.Enclosure.Size:0.##}");

        return new ConstraintResult(failed.Count == 0, passed, failed);
    }

    // ----------------------
    // Enclosure actions
    // ----------------------

    public async Task<List<ActionResultLine>> EnclosureSunriseAsync(int enclosureId)
    {
        var animals = await _db.Animals.Where(a => a.EnclosureId == enclosureId).ToListAsync();
        return animals.Select(a => new ActionResultLine(a.Id, a.Name, AnimalSunrise(a))).ToList();
    }

    public async Task<List<ActionResultLine>> EnclosureSunsetAsync(int enclosureId)
    {
        var animals = await _db.Animals.Where(a => a.EnclosureId == enclosureId).ToListAsync();
        return animals.Select(a => new ActionResultLine(a.Id, a.Name, AnimalSunset(a))).ToList();
    }

    public async Task<List<ActionResultLine>> EnclosureFeedingTimeAsync(int enclosureId)
    {
        var animals = await _db.Animals
            .Where(a => a.EnclosureId == enclosureId)
            .Select(a => a.Id)
            .ToListAsync();

        var result = new List<ActionResultLine>();
        foreach (var id in animals)
        {
            var a = await _db.Animals.FirstAsync(x => x.Id == id);
            var msg = await AnimalFeedingTimeAsync(id);
            result.Add(new ActionResultLine(id, a.Name, msg));
        }
        return result;
    }

    public async Task<ConstraintResult> EnclosureCheckConstraintsAsync(int enclosureId)
    {
        var enclosure = await _db.Enclosures.FirstOrDefaultAsync(e => e.Id == enclosureId);
        if (enclosure == null)
            return new ConstraintResult(false, new(), new() { "Enclosure not found." });

        var animals = await _db.Animals.Where(a => a.EnclosureId == enclosureId).ToListAsync();

        var passed = new List<string>();
        var failed = new List<string>();

        var used = animals.Sum(a => a.SpaceRequirement);
        if (used <= enclosure.Size) passed.Add($"Space OK: used {used:0.##} <= {enclosure.Size:0.##}");
        else failed.Add($"Space FAIL: used {used:0.##} > {enclosure.Size:0.##}");

        var maxReq = animals.Count == 0 ? SecurityLevel.Low : animals.Max(a => a.SecurityRequirement);
        if (enclosure.SecurityLevel >= maxReq) passed.Add($"Security OK: enclosure {enclosure.SecurityLevel} >= max animal requirement {maxReq}");
        else failed.Add($"Security FAIL: enclosure {enclosure.SecurityLevel} < max animal requirement {maxReq}");

        return new ConstraintResult(failed.Count == 0, passed, failed);
    }

    // ----------------------
    // Zoo actions
    // ----------------------

    public async Task<List<ActionResultLine>> ZooSunriseAsync()
    {
        var animals = await _db.Animals.ToListAsync();
        return animals.Select(a => new ActionResultLine(a.Id, a.Name, AnimalSunrise(a))).ToList();
    }

    public async Task<List<ActionResultLine>> ZooSunsetAsync()
    {
        var animals = await _db.Animals.ToListAsync();
        return animals.Select(a => new ActionResultLine(a.Id, a.Name, AnimalSunset(a))).ToList();
    }

    public async Task<List<ActionResultLine>> ZooFeedingTimeAsync()
    {
        var ids = await _db.Animals.Select(a => a.Id).ToListAsync();
        var result = new List<ActionResultLine>();
        foreach (var id in ids)
        {
            var a = await _db.Animals.FirstAsync(x => x.Id == id);
            var msg = await AnimalFeedingTimeAsync(id);
            result.Add(new ActionResultLine(id, a.Name, msg));
        }
        return result;
    }

    public async Task<ConstraintResult> ZooCheckConstraintsAsync()
    {
        var enclosures = await _db.Enclosures.Select(e => e.Id).ToListAsync();

        var passed = new List<string>();
        var failed = new List<string>();

        foreach (var encId in enclosures)
        {
            var r = await EnclosureCheckConstraintsAsync(encId);
            if (r.Ok) passed.Add($"Enclosure {encId}: OK");
            else failed.Add($"Enclosure {encId}: FAIL ({string.Join(" | ", r.Failed)})");
        }

        return new ConstraintResult(failed.Count == 0, passed, failed);
    }

    // reset=true => gooi alle enclosure links weg en maak opnieuw
    // reset=false => vul bestaande verblijven aan, maak nieuw als nodig
    public async Task<List<string>> ZooAutoAssignAsync(bool reset)
    {
        var log = new List<string>();

        var animals = await _db.Animals
            .Include(a => a.Enclosure)
            .ToListAsync();

        if (reset)
        {
            foreach (var a in animals) a.EnclosureId = null;
            await _db.SaveChangesAsync();
            log.Add("Reset: all animals unassigned from enclosures.");
        }

        var enclosures = await _db.Enclosures.ToListAsync();

        // helper: find/create enclosure by security level
        async Task<Enclosure> GetOrCreateEnclosure(SecurityLevel level)
        {
            var existing = enclosures.FirstOrDefault(e => e.SecurityLevel == level);
            if (existing != null) return existing;

            var created = new Enclosure
            {
                Name = $"{level} Security Enclosure",
                Climate = Climate.Temperate,
                HabitatType = HabitatType.Forest,
                SecurityLevel = level,
                Size = 500
            };

            _db.Enclosures.Add(created);
            await _db.SaveChangesAsync();
            enclosures = await _db.Enclosures.ToListAsync();

            log.Add($"Created enclosure: {created.Name} ({created.Size} m2).");
            return created;
        }

        // assign animals by required security, check capacity
        foreach (var a in animals)
        {
            if (a.EnclosureId != null) continue;

            var enc = await GetOrCreateEnclosure(a.SecurityRequirement);

            // capacity check
            var used = animals.Where(x => x.EnclosureId == enc.Id).Sum(x => x.SpaceRequirement);
            if (used + a.SpaceRequirement <= enc.Size)
            {
                a.EnclosureId = enc.Id;
                log.Add($"Assigned {a.Name} -> {enc.Name}");
            }
            else
            {
                // create extra enclosure
                var extra = new Enclosure
                {
                    Name = $"{a.SecurityRequirement} Security Enclosure (Extra)",
                    Climate = Climate.Temperate,
                    HabitatType = HabitatType.Forest,
                    SecurityLevel = a.SecurityRequirement,
                    Size = Math.Max(500, a.SpaceRequirement * 10)
                };
                _db.Enclosures.Add(extra);
                await _db.SaveChangesAsync();

                enclosures.Add(extra);
                a.EnclosureId = extra.Id;
                log.Add($"Created extra enclosure and assigned {a.Name} -> {extra.Name}");
            }
        }

        await _db.SaveChangesAsync();
        return log;
    }
}
