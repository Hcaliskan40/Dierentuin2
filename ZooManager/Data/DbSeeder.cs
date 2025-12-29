using Bogus;
using ZooManager.Models;

namespace ZooManager.Data;

public static class DbSeeder
{
    public static void Seed(ZooDbContext db)
    {
        // Already seeded?
        if (db.Animals.Any() || db.Enclosures.Any() || db.Categories.Any())
        {
            return;
        }

        // Categories
        var categories = new Faker<Category>()
            .RuleFor(c => c.Name, f => f.Commerce.Categories(1)[0])
            .Generate(5);

        // Enclosures
        var enclosures = new Faker<Enclosure>()
            .RuleFor(e => e.Name, f => $"{f.Address.City()} Enclosure")
            .RuleFor(e => e.Climate, f => f.PickRandom<Climate>())
            .RuleFor(e => e.HabitatType, f => f.PickRandom<HabitatType>())
            .RuleFor(e => e.SecurityLevel, f => f.PickRandom<SecurityLevel>())
            .RuleFor(e => e.Size, f => Math.Round(f.Random.Double(100, 1200), 2))
            .Generate(6);

        db.Categories.AddRange(categories);
        db.Enclosures.AddRange(enclosures);
        db.SaveChanges();

        // Animals
        var animals = new Faker<Animal>()
            .RuleFor(a => a.Name, f => f.Name.FirstName())
            .RuleFor(a => a.Species, f => f.Random.Word()) // veilig; Bogus Animal dataset verschilt soms per versie
            .RuleFor(a => a.Size, f => f.PickRandom<Size>())
            .RuleFor(a => a.DietaryClass, f => f.PickRandom<DietaryClass>())
            .RuleFor(a => a.ActivityPattern, f => f.PickRandom<ActivityPattern>())
            .RuleFor(a => a.SpaceRequirement, f => Math.Round(f.Random.Double(5, 80), 2))
            .RuleFor(a => a.SecurityRequirement, f => f.PickRandom<SecurityLevel>())
            .RuleFor(a => a.Category, f => f.Random.Bool(0.2f) ? null : f.PickRandom(categories))
            .RuleFor(a => a.Enclosure, f => f.Random.Bool(0.2f) ? null : f.PickRandom(enclosures))
            .RuleFor(a => a.Prey, _ => null)
            .Generate(25);

        // Simple prey assignment
        var carnivores = animals
            .Where(a => a.DietaryClass is DietaryClass.Carnivore or DietaryClass.Piscivore or DietaryClass.Insectivore)
            .ToList();

        var possiblePrey = animals
            .Except(carnivores)
            .ToList();

        var rnd = new Random();

        foreach (var predator in carnivores)
        {
            if (possiblePrey.Count == 0)
            {
                break;
            }

            if (rnd.NextDouble() < 0.4)
            {
                predator.Prey = possiblePrey[rnd.Next(0, possiblePrey.Count)];
            }
        }

        db.Animals.AddRange(animals);
        db.SaveChanges();
    }
}
