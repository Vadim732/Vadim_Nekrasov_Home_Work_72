namespace Delivery.Models;

public class Basket
{
    public int Id { get; set; }
    
    public int UserId { get; set; }
    public User User { get; set; }
    
    public int EstablishmentId { get; set; }
    public Establishment Establishment { get; set; }
    
    public ICollection<Dish> Dishes { get; set; } = new List<Dish>();
}