using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZooManager.Data;
using ZooManager.Models;
using ZooManager.Services;

namespace ZooManager.Controllers.Api;

[ApiController]
[Route("api/animals")]
public class AnimalsApiController : ControllerBase
{
    private readonly ZooDbContext _db;
    private readonly ZooLogicService _logic;

    public AnimalsApiController(ZooDbContext db, ZooLogicService logic)
    {
        _db = db;
        _logic = logic;
    }

    // GET api/animals?name=&species=&size=&diet=&activity=&security=&categoryId=&enclosureId=
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Animal>>> Get(
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

        if (!string.IsNullOrWhiteSpace(name)) q = q.Where(a => a.Name.Contains(name));
        if (!string.IsNullOrWhiteSpace(species)) q = q.Where(a => a.Species.Contains(species));
        if (size != null) q = q.Where(a => a.Size == size);
        if (diet != null) q = q.Where(a => a.DietaryClass == diet);
        if (activity != null) q = q.Where(a => a.ActivityPattern == activity);
        if (security != null) q = q.Where(a => a.SecurityRequirement == security);
        if (categoryId != null) q = q.Where(a => a.CategoryId == categoryId);
        if (enclosureId != null) q = q.Where(a => a.EnclosureId == enclosureId);

        return Ok(await q.ToListAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Animal>> GetById(int id)
    {
        var a = await _db.Animals
            .Include(x => x.Category)
            .Include(x => x.Enclosure)
            .Include(x => x.Prey)
            .FirstOrDefaultAsync(x => x.Id == id);

        return a == null ? NotFound() : Ok(a);
    }

    [HttpPost]
    public async Task<ActionResult<Animal>> Create(Animal a)
    {
        _db.Animals.Add(a);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = a.Id }, a);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Animal a)
    {
        if (id != a.Id) return BadRequest();

        _db.Entry(a).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var a = await _db.Animals.FindAsync(id);
        if (a == null) return NotFound();
        _db.Animals.Remove(a);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // ACTIONS
    [HttpPost("{id:int}/sunrise")]
    public async Task<ActionResult<string>> Sunrise(int id)
    {
        var a = await _db.Animals.FindAsync(id);
        if (a == null) return NotFound();
        return Ok(_logic.AnimalSunrise(a));
    }

    [HttpPost("{id:int}/sunset")]
    public async Task<ActionResult<string>> Sunset(int id)
    {
        var a = await _db.Animals.FindAsync(id);
        if (a == null) return NotFound();
        return Ok(_logic.AnimalSunset(a));
    }

    [HttpPost("{id:int}/feeding")]
    public async Task<ActionResult<string>> Feeding(int id)
        => Ok(await _logic.AnimalFeedingTimeAsync(id));

    [HttpPost("{id:int}/checkconstraints")]
    public async Task<ActionResult<ConstraintResult>> CheckConstraints(int id)
        => Ok(await _logic.AnimalCheckConstraintsAsync(id));
}
