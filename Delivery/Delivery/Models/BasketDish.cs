namespace Delivery.Models;

public class BasketDish
{
    public int Id { get; set; }
        
    public int BasketId { get; set; }
    public Basket Basket { get; set; }

    public int DishId { get; set; }
    public Dish Dish { get; set; }

    public int Quantity { get; set; }
}