using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebServer;
using WebServer.Models;

namespace WebServer.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /User/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /User/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == loginModel.Login && u.Password == loginModel.Password);
            

            if (user != null)
            {
                var userClaim = await _context.UserRoleClaims.FirstOrDefaultAsync(u => u.User_Id == user.User_Id);
                var userRole = await _context.Roles.FirstOrDefaultAsync(u => u.Role_Id == userClaim.Role_Id);
                // Аутентификация прошла успешно, устанавливаем cookie с идентификатором пользователя
                // и перенаправляем на защищенную страницу
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.PrimarySid, user.User_Id),
                    new Claim(ClaimTypes.Role, userRole.Role_Name)
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10) // Например, устанавливаем срок действия cookie в 1 час
                };
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                return RedirectToAction("Profile", "User");
            }
            else
            {
                // Пользователь не найден или неактивен, возвращаем представление с ошибкой
                ModelState.AddModelError(string.Empty, "Неправильный логин или пароль.");
                return View();
            }
        }

        // GET: /User/Profile
        [Authorize]
        public IActionResult Profile()  
        {
            var user = _context.Users.FirstOrDefault(u => u.Name == User.Identity.Name);
            return View(user);
        }

        // POST: /User/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
