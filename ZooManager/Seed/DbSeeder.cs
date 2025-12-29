using Bogus;
using Microsoft.EntityFrameworkCore;
using ZooManager.Data;
using ZooManager.Models;

namespace ZooManager.Seed;

public static class DbSeeder
{
    public static async Task SeedAsync(ZooDbContext db)
    {
        // Zorgt dat DB bestaat + migrations zijn toegepast
        await db.Database.MigrateAsync();

        // Niet opnieuw seeden als er al data is
        if (await db.Enclosures.AnyAsync() || await db.Animals.AnyAsync() || await db.Categories.AnyAsync())
            return;

        // Categories
        var categories = new List<Category>
        {
            new() { Name = "Mammals" },
            new() { Name = "Birds" },
            new() { Name = "Reptiles" },
            new() { Name = "Fish" },
            new() { Name = "Insects" }
        };
        db.Categories.AddRange(categories);

        // Enclosures
        var enclosureFaker = new Faker<Enclosure>()
            .RuleFor(e => e.Name, f => $"{f.Commerce.Color()} Enclosure")
            .RuleFor(e => e.Climate, f => f.PickRandom<Climate>())
            .RuleFor(e => e.HabitatType, f => f.PickRandom<HabitatType>())
            .RuleFor(e => e.SecurityLevel, f => f.PickRandom<SecurityLevel>())
            .RuleFor(e => e.Size, f => Math.Round(f.Random.Double(50, 800), 2));

        var enclosures = enclosureFaker.Generate(6);
        db.Enclosures.AddRange(enclosures);
        
        
        var speciesList = new[]
        {
            "Lion","Tiger","Elephant","Giraffe","Zebra","Bear","Wolf","Penguin","Eagle","Snake",
            "Crocodile","Turtle","Shark","Dolphin","Frog","Butterfly","Ant","Spider","Kangaroo","Rhino"
        };

        // Animals
        var animalFaker = new Faker<Animal>()
            .RuleFor(a => a.Name, f => f.Name.FirstName())
            .RuleFor(a => a.Species, f => f.PickRandom(speciesList))
            .RuleFor(a => a.Size, f => f.PickRandom<AnimalSize>())
            .RuleFor(a => a.DietaryClass, f => f.PickRandom<DietaryClass>())
            .RuleFor(a => a.ActivityPattern, f => f.PickRandom<ActivityPattern>())
            .RuleFor(a => a.Prey, f => f.Random.Bool(0.4f) ? f.PickRandom(speciesList) : null)
            .RuleFor(a => a.SpaceRequirement, f => Math.Round(f.Random.Double(2, 40), 2))
            .RuleFor(a => a.SecurityRequirement, f => f.PickRandom<SecurityLevel>())
            .RuleFor(a => a.Category, f => f.PickRandom(categories))
            .RuleFor(a => a.Enclosure, f => f.Random.Bool(0.7f) ? f.PickRandom(enclosures) : null);

        var animals = animalFaker.Generate(25);
        db.Animals.AddRange(animals);

        await db.SaveChangesAsync();
    }
}
