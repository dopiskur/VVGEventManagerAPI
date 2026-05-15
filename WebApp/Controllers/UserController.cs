using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApp.ViewModels;
using eventLib.Models;
using eventLib.Security;
using WebApp.Helpers;
using Microsoft.Extensions.Hosting;
using eventLib.Dal.API;

namespace exercise_13.Controllers
{
    public class UserController : Controller
    {
        private readonly IApi _api;

        public UserController(IApi api)
        {
            _api = api;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login(string returnUrl)
        {
            var loginVm = new LoginVM
            {
                ReturnUrl = returnUrl
            };

            return View(loginVm);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVm)
        {
            try
            {
                var result = await _api.UserLogin(new UserLogin
                {
                    Username = loginVm.Username,
                    Password = loginVm.Password
                });

                if (result == null || string.IsNullOrEmpty(result.Token))
                {
                    ModelState.AddModelError("", "Invalid username or password");
                    return View(loginVm);
                }

                this.SetJwtToken(result.Token);

                var claims = new List<Claim>
                {
                    new(ClaimTypes.Name, result.Username ?? loginVm.Username),
                    new(ClaimTypes.Role, result.RoleName ?? "User")
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    new AuthenticationProperties());

                if (loginVm.ReturnUrl != null)
                    return LocalRedirect(loginVm.ReturnUrl);

                if (result.RoleName == "Admin")
                    return RedirectToAction("Index", "EventManagement");
                if (result.RoleName == "User")
                    return RedirectToAction("Index", "Events");

                return View(loginVm);
            }
            catch (Exception ex)
            {
                var message = HttpContext.RequestServices.GetService<IHostEnvironment>()?.IsDevelopment() == true
                    ? ex.Message
                    : "Invalid username or password";
                ModelState.AddModelError("", message);
                return View(loginVm);
            }
        }

        public async Task<IActionResult> Logout()
        {
            this.ClearJwtToken();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserVM userVm)
        {
            try
            {
                var trimmedUsername = userVm.Username.Trim();
                var existing = await _api.UserGet(null, null, trimmedUsername);
                if (existing != null)
                    return BadRequest($"Username {trimmedUsername} already exists");

                var b64salt = AuthenticationProvider.GetSalt();
                var b64hash = AuthenticationProvider.GetHash(userVm.Password, b64salt);

                var user = new User
                {
                    Username = userVm.Username,
                    PwdHash = b64hash,
                    PwdSalt = b64salt,
                    FirstName = userVm.FirstName,
                    LastName = userVm.LastName,
                    Email = userVm.Email,
                    Phone = userVm.Phone,
                    UserRoleId = 2,
                };

                userVm.IDUser = await _api.UserRegister(user);
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
