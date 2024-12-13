using Delivery.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Delivery.Controllers;

public class EstablishmentController : Controller
{
    private readonly DeliveryContext _context;

    public EstablishmentController(DeliveryContext context)
    {
        _context = context;
    }
    
    public async Task<IActionResult> Index()
    {
        List<Establishment> establishments = await _context.Establishments.ToListAsync();
        return View(establishments);
    }

    public async Task<IActionResult> EstablishmentDetailsPage(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        
        Establishment? establishment = await _context.Establishments.Include(e => e.Dishes).FirstOrDefaultAsync(e => e.Id == id);
        
        if (establishment == null)
        {
            return NotFound();
        }
        return View(establishment);
    }
    
}