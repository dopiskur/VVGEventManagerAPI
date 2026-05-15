using eventLib.Dal;
using eventLib.Models;
using eventLib.Security;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using WebApp.Helpers;

namespace WebApp.Controllers
{
    public class UserManagementController : Controller
    {
        private readonly IApi _api;

        public UserManagementController(IApi api)
        {
            _api = api;
        }

        public async Task<ActionResult> Index()
        {
            var jwt = this.GetJwtToken();
            UserVM userVM = new UserVM();
            userVM.Users = await _api.UsersGet(jwt!);
            return View(userVM);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UserVM userVM)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        [Authorize]
        public async Task<ActionResult> Edit(int? idUser)
        {
            var jwt = this.GetJwtToken();
            User user = (await _api.UserGet(jwt, idUser, null))!;
            UserVM userVM = new UserVM();

            userVM.UserEdit.IDUser = user.IDUser;
            userVM.UserEdit.Username = user.Username;
            userVM.UserEdit.FirstName = user.FirstName;
            userVM.UserEdit.LastName = user.LastName;
            userVM.UserEdit.Email = user.Email;
            userVM.UserEdit.Phone = user.Phone;
            userVM.UserEdit.RoleName = user.RoleName;

            userVM.UserRoles = await _api.UserRolesGet(jwt!);
            return View(userVM);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(UserVM value)
        {
            try
            {
                var jwt = this.GetJwtToken();
                User user = (await _api.UserGet(jwt, value.UserEdit.IDUser, null))!;

                user.IDUser = value.UserEdit.IDUser;
                user.Username = value.UserEdit.Username;
                user.FirstName = value.UserEdit.FirstName;
                user.LastName = value.UserEdit.LastName;
                user.Email = value.UserEdit.Email;
                user.Phone = value.UserEdit.Phone;
                user.UserRoleId = value.UserEdit.UserRoleId;

                if (value.UserEdit.Password != null)
                {
                    var b64salt = AuthenticationProvider.GetSalt();
                    var b64hash = AuthenticationProvider.GetHash(value.UserEdit.Password, b64salt);
                    user.PwdSalt = b64salt;
                    user.PwdHash = b64hash;
                }

                await _api.UserUpdate(jwt!, user);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        [Authorize]
        public async Task<ActionResult> Delete(int? idUser)
        {
            var jwt = this.GetJwtToken();
            User value = (await _api.UserGet(jwt, idUser, null))!;
            return View(value);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirm(int? idUser)
        {
            try
            {
                var jwt = this.GetJwtToken();
                await _api.UserDelete(jwt!, idUser);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [Authorize]
        public async Task<ActionResult> EditProfile()
        {
            var jwt = this.GetJwtToken();
            string? username = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            User? user = await _api.UserGet(jwt, null, username);
            UserVM userVM = new UserVM();

            if (user == null)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Login", "User");
            }

            userVM.UserEdit.IDUser = user.IDUser;
            userVM.UserEdit.Username = user.Username;
            userVM.UserEdit.FirstName = user.FirstName;
            userVM.UserEdit.LastName = user.LastName;
            userVM.UserEdit.Email = user.Email;
            userVM.UserEdit.Phone = user.Phone;
            userVM.UserEdit.RoleName = user.RoleName;

            userVM.UserRoles = await _api.UserRolesGet(jwt!);
            return View(userVM);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditProfile(UserVM value)
        {
            try
            {
                var jwt = this.GetJwtToken();
                User user = (await _api.UserGet(jwt, value.UserEdit.IDUser, null))!;

                user.Username = value.UserEdit.Username;
                user.FirstName = value.UserEdit.FirstName;
                user.LastName = value.UserEdit.LastName;
                user.Email = value.UserEdit.Email;
                user.Phone = value.UserEdit.Phone;

                if (value.UserEdit.Password != null)
                {
                    var b64salt = AuthenticationProvider.GetSalt();
                    var b64hash = AuthenticationProvider.GetHash(value.UserEdit.Password, b64salt);
                    user.PwdSalt = b64salt;
                    user.PwdHash = b64hash;
                }

                await _api.UserUpdate(jwt!, user);
                return View();
            }
            catch
            {
                return View();
            }
        }
    }
}
