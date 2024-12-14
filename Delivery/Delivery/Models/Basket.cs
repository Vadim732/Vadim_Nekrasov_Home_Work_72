namespace Delivery.Models;

public class Basket
{
    public int Id { get; set; }
    
    public int UserId { get; set; }
    public User User { get; set; }
    
    public int EstablishmentId { get; set; }
    public Establishment Establishment { get; set; }
    public ICollection<BasketDish> BasketDishes { get; set; } = new List<BasketDish>();
    public double TotalPrice => BasketDishes.Sum(bd => bd.Dish.Price * bd.Quantity);
}