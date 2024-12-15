using System.ComponentModel.DataAnnotations;

namespace Delivery.Models;

public class Dish
{
    public int Id { get; set; }
    [Required (ErrorMessage = "Название блюда обязательно для заполнения.")]
    public string Name { get; set; }
    [Required (ErrorMessage = "Это поле обязательно для заполнения.")]
    [Url (ErrorMessage = "Введите валидный URL адрес")]
    public string Image { get; set; }
    [Required (ErrorMessage = "Стоимость блюда обязательна для заполнения.")]
    [Range(0.2, double.MaxValue)]
    public double Price { get; set; }
    [Required (ErrorMessage = "Описание блюда обязательно для заполнения.")]
    [MinLength(26, ErrorMessage = "Описание должно быть не короче 26 символов! Постарайтесь быть содержательнее))")]
    public string Description { get; set; }
    
    public int? UserId { get; set; }
    public User? User { get; set; }
    
    public int? EstablishmentId { get; set; }
    public Establishment? Establishments { get; set; }
}