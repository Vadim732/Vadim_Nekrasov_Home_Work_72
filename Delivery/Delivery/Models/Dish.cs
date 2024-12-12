using System.ComponentModel.DataAnnotations;

namespace Delivery.Models;

public class Dish
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    [Url]
    public string Image { get; set; }
    [Required]
    [Range(0.2, double.MaxValue)]
    public double Price { get; set; }
    [Required]
    [MinLength(26, ErrorMessage = "Описание должно быть не короче 26 символов! Постарайтесь быть содержательнее))")]
    public string Description { get; set; }
    
    public int? UserId { get; set; }
    public User? User { get; set; }
    
    public int? EstablishmentId { get; set; }
    public Establishment? Establishments { get; set; }
}