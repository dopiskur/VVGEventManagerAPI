using eventLib.Dal;
using eventLib.Models;
using WebApp.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers.API
{
    [Route("api/Event")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class EventApiController : ControllerBase
    {
        private readonly IRepository _repository;

        public EventApiController(IRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("All")]
        [Authorize(Roles = "Admin")]
        public ActionResult<IList<Event>> EventsGet(string? search)
        {
            return Ok(_repository.EventsGet(search ?? string.Empty));
        }

        [HttpGet("Browse")]
        [Authorize(Roles = "Admin,User")]
        public ActionResult<IList<Event>> MyEventsGet(string? search)
        {
            return Ok(_repository.MyEventsGet(search ?? string.Empty));
        }

        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public ActionResult<Event> EventGet(int idEvent)
        {
            var evt = _repository.EventGet(idEvent);
            if (evt == null)
                return NotFound();
            return Ok(evt);
        }

        [HttpGet("Types")]
        [Authorize(Roles = "Admin,User")]
        public ActionResult<IList<EventType>> EventTypesGet()
        {
            return Ok(_repository.EventTypesGet());
        }

        [HttpGet("Performers")]
        [Authorize(Roles = "Admin,User")]
        public ActionResult<IList<EventPerformer>> EventPerformersGet(int eventID)
        {
            return Ok(_repository.EventPerformersGet(eventID));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult<int> EventAdd([FromBody] Event value)
        {
            return Ok(_repository.EventAdd(value));
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public IActionResult EventUpdate([FromBody] Event value)
        {
            _repository.EventUpdate(value);
            return Ok();
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public IActionResult EventDelete(int idEvent)
        {
            _repository.EventDelete(idEvent);
            return Ok();
        }

        [HttpPost("Performers")]
        [Authorize(Roles = "Admin")]
        public IActionResult EventPerformerAdd([FromBody] EventPerformerLinkRequest request)
        {
            _repository.EventPerformerAdd(request.EventID, request.PerformerID);
            return Ok();
        }

        [HttpDelete("Performers")]
        [Authorize(Roles = "Admin")]
        public IActionResult EventPerformerDelete(int eventID, int performerID)
        {
            _repository.EventPerformerDelete(eventID, performerID);
            return Ok();
        }
    }
}
