using System.ComponentModel.DataAnnotations;

namespace ZooManager.Models;

public class Category
{
    public int Id { get; set; }          // <-- int, auto identity in SQL Server
    public string Name { get; set; } = string.Empty;

    public ICollection<Animal> Animals { get; set; } = new List<Animal>();
}
