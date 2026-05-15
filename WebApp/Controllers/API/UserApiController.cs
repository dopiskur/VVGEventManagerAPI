using eventLib.Dal;
using eventLib.Models;
using eventLib.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers.API
{
    [Route("api/User")]
    [ApiController]
    public class UserApiController : ControllerBase
    {
        private readonly IRepository _repository;

        public UserApiController(IRepository repository)
        {
            _repository = repository;
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public ActionResult<UserLoginResult> Login([FromBody] UserLogin value)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = _repository.UserGet(null, value.Username);
                if (user == null || !AuthenticationProvider.VerifyHash(user.PwdHash, user.PwdSalt, value.Password))
                    return Unauthorized("Invalid username or password");

                var token = AuthenticationProvider.CreateToken(user.Username!, user.RoleName ?? "User");
                return Ok(new UserLoginResult
                {
                    IDUser = user.IDUser,
                    Username = user.Username,
                    RoleName = user.RoleName,
                    Token = token
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("Register")]
        [AllowAnonymous]
        public ActionResult<int> Register([FromBody] User user)
        {
            try
            {
                if (_repository.UserGet(null, user.Username) != null)
                    return BadRequest($"Username {user.Username} already exists");

                if (string.IsNullOrEmpty(user.PwdHash) || string.IsNullOrEmpty(user.PwdSalt))
                    return BadRequest("Password hash and salt are required.");

                if (user.UserRoleId == null)
                    user.UserRoleId = 2;

                var id = _repository.UserAdd(user);
                return Ok(id);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("All")]
        [Authorize(Roles = "Admin", AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult<IList<User>> UsersGet()
        {
            return Ok(_repository.UsersGet());
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult<User> UserGet(int? idUser, string? username)
        {
            var user = _repository.UserGet(idUser, username);
            if (user == null)
                return NotFound();
            return Ok(user);
        }

        [HttpPut]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult UserUpdate([FromBody] User user)
        {
            _repository.UserUpdate(user);
            return Ok();
        }

        [HttpDelete]
        [Authorize(Roles = "Admin", AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult UserDelete(int? idUser)
        {
            _repository.UserDelete(idUser);
            return Ok();
        }

        [HttpGet("Roles")]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult<IList<UserRole>> UserRolesGet()
        {
            return Ok(_repository.UserRolesGet());
        }
    }
}
