using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZooManager.Data;

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
            .Include(a => a.Enclosure)
            .ToListAsync();

        return View(animals);
    }

    // GET: /Animals/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var animal = await _db.Animals
            .Include(a => a.Enclosure)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (animal == null) return NotFound();

        return View(animal);
    }
}