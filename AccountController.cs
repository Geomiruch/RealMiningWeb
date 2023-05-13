
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using User_Story.Data;
using User_Story.Models;

namespace User_Story.Controllers
{
    public class AccountController : Controller
    {
        private Context db;
        IHostingEnvironment Environment;
        public AccountController(Context context, IHostingEnvironment _environment)
        {
            db = context;
           Environment = _environment;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Login == model.Login && u.Password == model.Password);
                if (user != null)
                {
                    await Authenticate(user); // аутентификация

                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult Register()
        {
            ViewBag.Roles = new SelectList(db.Roles, "ID", "Name");
            return View();
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(RegisterModel model)
        {
                User user = await db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Login == model.Login);
                if (user != null)
                {
                    user.Password = model.Password;
                    db.Users.Update(user);
                    db.SaveChanges();
                    return RedirectToAction("Login", "Account");
                }
                ModelState.AddModelError("", "No user with this login");
                return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model, IFormFile file)
        {
            User user = await db.Users.FirstOrDefaultAsync(u => u.Login == model.Login);
            if (user == null)
            {
                string avatar = "";
                //TODO ADD DEFAULT VALUE FOR ROLE ID CLIENT --- NOTE REGISTER ONLY FOR CLIENTS...
                if (file == null)
                    avatar = "/img/men_avatar.png";

                User registerUser = new User {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Avatar = avatar,
                    Login = model.Login,
                    Password = model.Password,
                    RoleID = model.RoleID
                };

                // добавляем пользователя в бд
                db.Users.Add(registerUser);
                await db.SaveChangesAsync();

                if(file != null)
                {
                    string wwwPath = this.Environment.WebRootPath;
                    string contentPath = this.Environment.ContentRootPath;

                    User user2 = db.Users.First(x => x.Login.Equals(registerUser.Login));

                    string path = Path.Combine(this.Environment.WebRootPath, "Images/Users/" + user2.ID);
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    string fileName = Path.GetFileName(file.FileName);
                    using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    user2.Avatar = "/Images/Users/" + user2.ID + "/" + fileName;

                    db.Users.Update(user2);
                    db.SaveChanges();
                }

                return RedirectToAction("Login", "Account");
            }
            else
            {
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
                return View(model);
            }
        }

        private async Task Authenticate(User user)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role?.Name)
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}
