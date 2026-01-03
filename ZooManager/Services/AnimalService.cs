using ZooManager.Models;

namespace ZooManager.Services;

public class AnimalService
{
    public string Sunrise(Animal animal)
    {
        return animal.ActivityPattern switch
        {
            ActivityPattern.Nocturnal => $"{animal.Name} goes to sleep 🌙",
            ActivityPattern.Diurnal => $"{animal.Name} wakes up ☀️",
            ActivityPattern.Cathemeral => $"{animal.Name} stays active 🔁",
            _ => "Unknown activity pattern"
        };
    }

    public string Sunset(Animal animal)
    {
        return animal.ActivityPattern switch
        {
            ActivityPattern.Nocturnal => $"{animal.Name} wakes up 🌙",
            ActivityPattern.Diurnal => $"{animal.Name} goes to sleep ☀️",
            ActivityPattern.Cathemeral => $"{animal.Name} stays active 🔁",
            _ => "Unknown activity pattern"
        };
    }

    public string FeedingTime(Animal animal)
    {
        if (animal.Prey != null)
        {
            return $"{animal.Name} eats {animal.Prey.Name} 🩸";
        }

        return animal.DietaryClass switch
        {
            DietaryClass.Carnivore => $"{animal.Name} eats meat 🥩",
            DietaryClass.Herbivore => $"{animal.Name} eats plants 🌿",
            DietaryClass.Omnivore => $"{animal.Name} eats plants and meat 🍖🌱",
            DietaryClass.Insectivore => $"{animal.Name} eats insects 🐜",
            DietaryClass.Piscivore => $"{animal.Name} eats fish 🐟",
            _ => "Unknown diet"
        };
    }

    public List<string> CheckConstraints(Animal animal)
    {
        var results = new List<string>();

        if (animal.Enclosure == null)
            results.Add("❌ No enclosure assigned");
        else
            results.Add("✅ Enclosure assigned");

        if (animal.Enclosure != null)
        {
            if (animal.Enclosure.SecurityLevel < animal.SecurityRequirement)
                results.Add("❌ Enclosure security level too low");
            else
                results.Add("✅ Security level sufficient");
        }

        return results;
    }
}
