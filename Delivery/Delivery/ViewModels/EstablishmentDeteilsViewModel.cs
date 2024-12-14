using Delivery.Models;

namespace Delivery.ViewModels;

public class EstablishmentDeteilsViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Image { get; set; }
    public List<Dish> Dishes { get; set; }
    public Basket Basket { get; set; }
}