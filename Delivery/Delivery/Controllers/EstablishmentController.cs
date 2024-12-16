using Delivery.Models;
using Delivery.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Delivery.Controllers;

public class EstablishmentController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly DeliveryContext _context;

    public EstablishmentController(UserManager<User> userManager, SignInManager<User> signInManager,
        DeliveryContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
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
        var establishment = await _context.Establishments
            .Include(e => e.Dishes)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (establishment == null)
        {
            return NotFound();
        }
        var user = await _userManager.GetUserAsync(User); 
        Basket basket = null;
        if (user != null)
        {
            basket = await _context.Baskets
                .Include(b => b.BasketDishes)
                .FirstOrDefaultAsync(b => b.UserId == user.Id && b.EstablishmentId == id);
        }
        var model = new EstablishmentDeteilsViewModel()
        {
            Id = establishment.Id,
            Name = establishment.Name,
            Description = establishment.Description,
            Image = establishment.Image,
            Dishes = new List<Dish>(_context.Dishes.Where(d => d.EstablishmentId == establishment.Id)),
            Basket = basket
        };

        return View(model);
    }

    [Authorize(Roles = "admin")]
    public IActionResult CreateEstablishment()
    {
        return View(new EstablishmentViewModel());
    }

    [Authorize(Roles = "admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateEstablishment(EstablishmentViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null || !await _userManager.IsInRoleAsync(user, "admin"))
        {
            return Forbid();
        }

        if (ModelState.IsValid)
        {
            Establishment establishment = new Establishment
            {
                Name = model.Name,
                Image = model.Image,
                Description = model.Description
            };

            _context.Establishments.Add(establishment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        return View(model);
    }

    [Authorize(Roles = "admin")]
    [HttpGet]
    public async Task<IActionResult> EditEstablishment(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        Establishment? establishment = await _context.Establishments.FirstOrDefaultAsync(e => e.Id == id);
        ;
        if (establishment == null)
        {
            return NotFound();
        }

        EstablishmentViewModel model = new EstablishmentViewModel
        {
            Name = establishment.Name,
            Image = establishment.Image,
            Description = establishment.Description
        };

        return View(model);
    }

    [Authorize(Roles = "admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditEstablishment(EstablishmentViewModel model, int id)
    {
        if (ModelState.IsValid)
        {
            Establishment? existingEstablishment = await _context.Establishments
                .Where(e => e.Name == model.Name && e.Id != id)
                .FirstOrDefaultAsync();

            if (existingEstablishment != null)
            {
                ModelState.AddModelError("Name", "Заведение с таким названием уже существует, выберите другое.");
                return View(model);
            }

            Establishment? establishment = await _context.Establishments.FindAsync(id);
            if (establishment != null)
            {
                establishment.Name = model.Name;
                establishment.Image = model.Image;
                establishment.Description = model.Description;
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
        }

        return View(model);
    }


    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteEstablishment(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        Establishment? establishment = await _context.Establishments.FindAsync(id);
        if (establishment == null)
        {
            return NotFound();
        }

        return View(establishment);
    }

    [Authorize(Roles = "admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        Establishment? establishment = await _context.Establishments.FindAsync(id);
        if (establishment != null)
        {
            _context.Establishments.Remove(establishment);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Index");
    }

    [Authorize(Roles = "admin")]
    [HttpGet]
    public IActionResult CreateDish(int establishmentId)
    {
        ViewBag.EstablishmentId = establishmentId;
        return View();
    }

    [Authorize(Roles = "admin")]
    [HttpPost]
    public async Task<IActionResult> CreateDish(int establishmentId, Dish model)
    {
        if (ModelState.IsValid)
        {
            Dish dish = new Dish
            {
                Name = model.Name,
                Description = model.Description,
                Image = model.Image,
                Price = model.Price,
                EstablishmentId = establishmentId
            };

            _context.Dishes.Add(dish);
            await _context.SaveChangesAsync();
            return RedirectToAction("EstablishmentDetailsPage", new { id = establishmentId });
        }

        ViewBag.EstablishmentId = establishmentId;
        return View(model);
    }

    [Authorize(Roles = "admin")]
    [HttpGet]
    public async Task<IActionResult> EditDish(int? dishId)
    {
        Dish? dish = await _context.Dishes.FindAsync(dishId);
        if (dish == null)
        {
            return NotFound();
        }
        return View(dish);
    }

    [Authorize(Roles = "admin")]
    [HttpPost]
    public async Task<IActionResult> EditDish(int dishId, Dish model)
    {
        if (ModelState.IsValid)
        {
            Dish? dish = await _context.Dishes.FindAsync(dishId);
            if (dish != null)
            {
                dish.Name = model.Name;
                dish.Description = model.Description;
                dish.Image = model.Image;
                dish.Price = model.Price;
                await _context.SaveChangesAsync();
                return RedirectToAction("EstablishmentDetailsPage", new { id = dish.EstablishmentId });
            }
        }
        return View(model);
    }

    [Authorize(Roles = "admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteDish(int? dishId)
    {
        Dish? dish = await _context.Dishes.FindAsync(dishId);
        if (dish != null)
        {
            int? establishmentId = dish.EstablishmentId;
            _context.Dishes.Remove(dish);
            await _context.SaveChangesAsync();
        
            return RedirectToAction("EstablishmentDetailsPage", new { id = establishmentId });
        }
        return NotFound();
    }
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddToBasket(int dishId, int establishmentId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Json(new { success = false, message = "User not authorized" });
        }
        var dish = await _context.Dishes.FirstOrDefaultAsync(d => d.Id == dishId);
        if (dish == null)
        {
            return Json(new { success = false, message = "Dish not found" });
        }

        var basket = await _context.Baskets
            .Include(b => b.BasketDishes)
            .ThenInclude(basketDish => basketDish.Dish)
            .FirstOrDefaultAsync(b => b.UserId == user.Id && b.EstablishmentId == establishmentId);

        if (basket == null)
        {
            basket = new Basket
            {
                UserId = user.Id,
                EstablishmentId = establishmentId,
                BasketDishes = new List<BasketDish>()
            };
            _context.Baskets.Add(basket);
            await _context.SaveChangesAsync();
        }

        var dishInBasket = basket.BasketDishes.FirstOrDefault(bd => bd.DishId == dishId);
        if (dishInBasket != null)
        {
            dishInBasket.Quantity++;
        }
        else
        {
            basket.BasketDishes.Add(new BasketDish
            {
                DishId = dishId,
                Quantity = 1
            });
        }
        await _context.SaveChangesAsync();
        var basketResponse = new
        {
            success = true,
            BasketDishes = basket.BasketDishes
                .Where(bd => bd.Dish != null)
                .Select(bd => new
                {
                    DishName = bd.Dish.Name,
                    Quantity = bd.Quantity,
                    Price = bd.Dish.Price,
                    TotalPrice = bd.Quantity * bd.Dish.Price,
                    DishId = bd.DishId
                }).ToList(),
            TotalPrice = basket.BasketDishes
                .Where(bd => bd.Dish != null)
                .Sum(bd => bd.Quantity * bd.Dish.Price)
        };

        return Json(basketResponse);
    }
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> RemoveFromBasket(int dishId, int establishmentId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Json(new { success = false, message = "User not authorized" });
        }

        var basket = await _context.Baskets
            .Include(b => b.BasketDishes).ThenInclude(basketDish => basketDish.Dish)
            .FirstOrDefaultAsync(b => b.UserId == user.Id && b.EstablishmentId == establishmentId);

        if (basket != null)
        {
            var dishInBasket = basket.BasketDishes.FirstOrDefault(bd => bd.DishId == dishId);
            if (dishInBasket != null)
            {
                if (dishInBasket.Quantity > 1)
                {
                    dishInBasket.Quantity--;
                }
                else
                {
                    basket.BasketDishes.Remove(dishInBasket);
                }
                await _context.SaveChangesAsync();
            }
        }

        var basketResponse = new
        {
            success = true,
            BasketDishes = basket.BasketDishes.Select(bd => new
            {
                DishName = bd.Dish.Name,
                Quantity = bd.Quantity,
                Price = bd.Dish.Price,
                TotalPrice = bd.Quantity * bd.Dish.Price,
                DishId = bd.DishId
            }).ToList(),
            TotalPrice = basket.BasketDishes.Sum(bd => bd.Quantity * bd.Dish.Price)
        };

        return Json(basketResponse);
    }
}