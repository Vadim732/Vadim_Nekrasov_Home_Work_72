using Delivery.Models;
using Delivery.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Delivery.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly DeliveryContext _context;

    public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, DeliveryContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
    }
    
    [Authorize]
    public async Task<IActionResult> Profile()
    {
        User user = await _userManager.GetUserAsync(User);
        if (user != null)
        {
            return View(user);
        }
        return RedirectToAction("Login", "Account");
    }
    
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            User user = await _userManager.FindByEmailAsync(model.Identifier) ?? await _userManager.FindByNameAsync(model.Identifier);
        
            if (user != null)
            {
                SignInResult result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    return RedirectToAction("Index", "Establishment");
                }
            }
            
            ModelState.AddModelError("", "Неверный логин или пароль!");
        }

        return View(model);
    }
    
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var existingUserEmail = await _userManager.FindByEmailAsync(model.Email);
            if (existingUserEmail != null)
            {
                ViewBag.ErrorMessage = "Ошибка: Этот адрес электронной почты уже используется другим пользователем!";
                return View(model);
            }
            
            var existingUserName = await _userManager.FindByNameAsync(model.UserName);
            if (existingUserName != null)
            {
                ViewBag.ErrorMessage = "Ошибка: Этот логин уже используется другим пользователем!";
                return View(model);
            }
            
            var currentDate = DateTime.UtcNow;
            var userAge = currentDate.Year - model.DateOfBirth.Year;
            if (model.DateOfBirth > currentDate.AddYears(-userAge)) 
            {
                userAge--;
            }
            if (userAge < 18)
            {
                ViewBag.ErrorMessage = "Ошибка: Нельзя зарегистрироваться пользователям моложе 18 лет!";
                return View(model);
            }

            User user = new User
            {
                UserName = model.UserName,
                Email = model.Email,
                Avatar = model.Avatar,
                DateOfBirth = model.DateOfBirth.ToUniversalTime()
                
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "user");
                await _signInManager.SignInAsync(user, false);
                return RedirectToAction("Index", "Establishment");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return View(model);
    }
    
    [HttpGet]
    [Authorize (Roles = "admin")]
    public IActionResult Index()
    {
        List<User> users = _userManager.Users.ToList();
        return View(users);
    }
    
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Edit()
    {
        User user = await _userManager.GetUserAsync(User);
        var roles = await _userManager.GetRolesAsync(user);
        if (user.Id == 1) 
        {
            ViewBag.ErrorMessage = "Ошибка: Пользователь с ID 1 не может быть отредактирован!";
            return RedirectToAction("Profile", "Account");
        }
        var model = new EditViewModel
        {
            UserName = user.UserName,
            Email = user.Email,
            Avatar = user.Avatar,
            DateOfBirth = user.DateOfBirth
        };
        return View(model);
    }
    
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditViewModel model)
    {
        if (ModelState.IsValid)
        {
            User user = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(user);
            if (user.Id == 1)
            {
                ViewBag.ErrorMessage = "Ошибка: Пользователь с ID 1 не может быть отредактирован!";
                return RedirectToAction("Profile", "Account");
            }
            if (user != null)
            {
                var existingUserEmail = await _userManager.FindByEmailAsync(model.Email);
                if (existingUserEmail != null && existingUserEmail.Id != user.Id)
                {
                    ViewBag.ErrorMessage = "Ошибка: Этот адрес электронной почты уже используется другим пользователем!";
                    return View(model);
                }
            
                var existingUserName = await _userManager.FindByNameAsync(model.UserName);
                if (existingUserName != null && existingUserName.Id != user.Id)
                {
                    ViewBag.ErrorMessage = "Ошибка: Этот логин уже используется другим пользователем!";
                    return View(model);
                }
            
                var currentDate = DateTime.Now;
                var userAge = currentDate.Year - model.DateOfBirth.Year;
                if (model.DateOfBirth > currentDate.AddYears(-userAge)) 
                {
                    userAge--;
                }
                if (userAge < 18)
                {
                    ViewBag.ErrorMessage = "Ошибка: Нельзя зарегистрироваться пользователям моложе 18 лет!";
                    return View(model);
                }
                
                user.UserName = model.UserName;
                user.Email = model.Email;
                user.Avatar = model.Avatar;
                user.DateOfBirth = DateTime.SpecifyKind(model.DateOfBirth, DateTimeKind.Utc);

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Profile", "Account");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
        }
    
        return View(model);
    }
    
    [HttpGet]
    [Authorize (Roles = "admin")]
    public IActionResult RegisterUser()
    {
        return View();
    }
    
    [HttpPost]
    [Authorize (Roles = "admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegisterUser(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var existingUserEmail = await _userManager.FindByEmailAsync(model.Email);
        if (existingUserEmail != null)
        {
            ViewBag.ErrorMessage = "Ошибка: Этот адрес электронной почты уже используется другим пользователем!";
            return View(model);
        }
    
        var existingUserName = await _userManager.FindByNameAsync(model.UserName);
        if (existingUserName != null)
        {
            ViewBag.ErrorMessage = "Ошибка: Этот логин уже используется другим пользователем!";
            return View(model);
        }
        
        var currentDate = DateTime.UtcNow;
        var userAge = currentDate.Year - model.DateOfBirth.Year;
        if (model.DateOfBirth > currentDate.AddYears(-userAge)) 
        {
            userAge--;
        }
        if (userAge < 18)
        {
            ViewBag.ErrorMessage = "Ошибка: Нельзя зарегистрироваться пользователям моложе 18 лет!";
            return View(model);
        }
        
        var user = new User
        {
            UserName = model.UserName,
            Email = model.Email,
            Avatar = model.Avatar,
            DateOfBirth = model.DateOfBirth.ToUniversalTime()
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "user");
            return RedirectToAction("Index", "Account");
        }
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(model);
    }
    
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteUser(int userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return NotFound($"Пользователь с ID {userId} не найден.");
        }
        var roles = await _userManager.GetRolesAsync(user);
        if (roles.Contains("admin"))
        {
            ViewBag.ErrorMessage = "Нельзя удалить администратора!";
            return RedirectToAction("Index", "Account");
        }
        var result = await _userManager.DeleteAsync(user);
        if (result.Succeeded)
        {
            return RedirectToAction("Index", "Account");
        }
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
        return RedirectToAction("Index", "Account");
    }
    
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> GrantAdminRole(int userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return NotFound($"Пользователь с ID {userId} не найден.");
        }
        var roles = await _userManager.GetRolesAsync(user);
        if (roles.Contains("admin"))
        {
            return RedirectToAction("Index", "Account");
        }
        var result = await _userManager.AddToRoleAsync(user, "admin");
        if (result.Succeeded)
        {
            return RedirectToAction("Index", "Account");
        }
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
        return RedirectToAction("Index", "Account");
    }
    
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> RevokeAdminRole(int userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return NotFound($"Пользователь с ID {userId} не найден.");
        }
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser.Id == userId)
        {
            return BadRequest("Ошибка: Администратор не может удалить свою роль.");
        }
        if (userId == 1)
        {
            return BadRequest("Ошибка: Пользователь с ID 1 не может быть лишён роли администратора.");
        }
    
        var roles = await _userManager.GetRolesAsync(user);
        if (!roles.Contains("admin"))
        {
            return RedirectToAction("Index", "Account");
        }
    
        var resultRemoveAdmin = await _userManager.RemoveFromRoleAsync(user, "admin");
        var resultAddUser = await _userManager.AddToRoleAsync(user, "user");
    
        if (resultRemoveAdmin.Succeeded && resultAddUser.Succeeded)
        {
            return RedirectToAction("Index", "Account");
        }
    
        foreach (var error in resultRemoveAdmin.Errors.Concat(resultAddUser.Errors))
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
    
        return RedirectToAction("Index", "Account");
    }

    
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login");
    }
}