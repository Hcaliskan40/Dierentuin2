namespace ZooManager.Models;

public enum Size
{
    Microscopic,
    VerySmall,
    Small,
    Medium,
    Large,
    VeryLarge
}

public enum DietaryClass
{
    Carnivore,
    Herbivore,
    Omnivore,
    Insectivore,
    Piscivore
}

public enum ActivityPattern
{
    Diurnal,
    Nocturnal,
    Cathemeral
}

public enum SecurityLevel
{
    Low,
    Medium,
    High
}

public enum Climate
{
    Tropical,
    Temperate,
    Arctic
}

[System.Flags]

public enum HabitatType
{
    Grassland,
    Forest,
    Savanna,
    Jungle,
    Desert,
    Arctic,
    Aquatic
}