using Microsoft.AspNetCore.Mvc;

namespace Delivery.Controllers;

public class EstablishmentController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}