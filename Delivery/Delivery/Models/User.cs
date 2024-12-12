using Microsoft.AspNetCore.Identity;

namespace Delivery.Models;

public class User : IdentityUser<int>
{
    public string Avatar { get; set; }
    public DateTime DateOfBirth { get; set; }
    
    public ICollection<Dish>? Dishes { get; set; }
    
    public int DishCount { get { return Dishes?.Count() ?? 0; } }
}