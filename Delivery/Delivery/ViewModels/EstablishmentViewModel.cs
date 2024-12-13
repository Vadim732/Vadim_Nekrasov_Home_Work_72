using System.ComponentModel.DataAnnotations;
using Delivery.Models;

namespace Delivery.ViewModels;

public class EstablishmentViewModel
{
    [Required(ErrorMessage = "Название заведения обязательно для заполнения.")]
    [StringLength(30, MinimumLength = 2, ErrorMessage = "Название заведения содержать от 2 до 30 символов.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "URL изображения обязательно для заполнения.")]
    [Url(ErrorMessage = "Введите корректный URL для изображения.")]
    public string Image { get; set; }

    [Required(ErrorMessage = "Описание обязательно для заполнения.")]
    [MinLength(60, ErrorMessage = "Описание должно быть не короче 60 символов! Расскажите о своём заведении подробнее с:))")]
    public string Description { get; set; }
    
}