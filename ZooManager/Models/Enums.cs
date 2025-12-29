namespace ZooManager.Models;

public enum AnimalSize { Microscopic, VerySmall, Small, Medium, Large, VeryLarge }
public enum DietaryClass { Carnivore, Herbivore, Omnivore, Insectivore, Piscivore }
public enum ActivityPattern { Diurnal, Nocturnal, Cathemeral }
public enum Climate { Tropical, Temperate, Arctic }

[System.Flags]
public enum HabitatType { Forest = 1, Aquatic = 2, Desert = 4, Grassland = 8 }

public enum SecurityLevel { Low, Medium, High }