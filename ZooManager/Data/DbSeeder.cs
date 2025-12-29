using Bogus;
using ZooManager.Models;

namespace ZooManager.Data;

public static class DbSeeder
{
    public static void Seed(ZooDbContext db)
    {
        // Database al gevuld? Stop.
        if (db.Animals.Any() || db.Enclosures.Any() || db.Categories.Any())
        {
            return;
        }

        // Categories
        var categoryFaker = new Faker<Category>()
            .RuleFor(c => c.Name, f => f.Commerce.Categories(1)[0]);

        var categories = categoryFaker.Generate(5);

        // Enclosures
        var enclosureFaker = new Faker<Enclosure>()
            .RuleFor(e => e.Name, f => $"{f.Address.City()} Enclosure")
            .RuleFor(e => e.Climate, f => f.PickRandom<Climate>())
            .RuleFor(e => e.HabitatType, f => f.PickRandom<HabitatType>())
            .RuleFor(e => e.SecurityLevel, f => f.PickRandom<SecurityLevel>())
            .RuleFor(e => e.Size, f => Math.Round(f.Random.Double(100, 1200), 2));

        var enclosures = enclosureFaker.Generate(6);

        db.Categories.AddRange(categories);
        db.Enclosures.AddRange(enclosures);
        db.SaveChanges();

        // Animals
        var animalFaker = new Faker<Animal>()
            .RuleFor(a => a.Name, f => f.Name.FirstName())
            .RuleFor(a => a.Species, f => f.Animal.Type()) // Bogus has f.Animal.Type()
            .RuleFor(a => a.Size, f => f.PickRandom<Size>())
            .RuleFor(a => a.DietaryClass, f => f.PickRandom<DietaryClass>())
            .RuleFor(a => a.ActivityPattern, f => f.PickRandom<ActivityPattern>())
            .RuleFor(a => a.SpaceRequirement, f => Math.Round(f.Random.Double(5, 80), 2))
            .RuleFor(a => a.SecurityRequirement, f => f.PickRandom<SecurityLevel>())
            .RuleFor(a => a.Category, f => f.Random.Bool(0.2f) ? null : f.PickRandom(categories))   // 20% null
            .RuleFor(a => a.Enclosure, f => f.Random.Bool(0.2f) ? null : f.PickRandom(enclosures)) // 20% null
            .RuleFor(a => a.Prey, _ => null); // zetten we later

        var animals = animalFaker.Generate(25);

        // Prey instellen (simpel): carnivores krijgen soms prey
        var carnivores = animals.Where(a =>
            a.DietaryClass == DietaryClass.Carnivore ||
            a.DietaryClass == DietaryClass.Piscivore ||
            a.DietaryClass == DietaryClass.Insectivore).ToList();

        var possiblePrey = animals.Except(carnivores).ToList();

        var rnd = new Random();
        foreach (var predator in carnivores)
        {
            if (possiblePrey.Count == 0) break;

            // 40% kans dat hij prey heeft
            if (rnd.NextDouble() < 0.4)
            {
                predator.Prey = possiblePrey[rnd.Next(possiblePrey.Count)];
            }
        }

        db.Animals.AddRange(animals);
        db.SaveChanges();
    }
}
