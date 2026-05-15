using eventLib.Models;
using WebApp.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using eventLib.Dal.Data;

namespace WebApp.Controllers.API
{
    [Route("api/EventRegistration")]
    [ApiController]
    [Authorize(Roles = "Admin,User", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class EventRegistrationApiController : ControllerBase
    {
        private readonly IRepository _repository;

        public EventRegistrationApiController(IRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public ActionResult<IList<EventRegistration>> EventRegistrationsGet(int? userID, string? search)
        {
            return Ok(_repository.EventRegistrationsGet(userID, search));
        }

        [HttpPost]
        public IActionResult EventRegistrationAdd([FromBody] EventRegistrationRequest request)
        {
            _repository.EventRegistrationAdd(request.EventID, request.UserID);
            return Ok();
        }

        [HttpDelete]
        public IActionResult EventRegistrationDelete(int eventID, int userID)
        {
            _repository.EventRegistrationDelete(eventID, userID);
            return Ok();
        }
    }
}
