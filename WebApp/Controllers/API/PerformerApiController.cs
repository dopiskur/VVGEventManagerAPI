using eventLib.Dal;
using eventLib.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers.API
{
    [Route("api/Performer")]
    [ApiController]
    [Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PerformerApiController : ControllerBase
    {
        private readonly IRepository _repository;

        public PerformerApiController(IRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("All")]
        public ActionResult<IList<Performer>> PerformersGet(string? search)
        {
            return Ok(_repository.PerformersGet(search));
        }

        [HttpGet]
        public ActionResult<Performer> PerformerGet(int idPerformer)
        {
            var performer = _repository.PerformerGet(idPerformer);
            if (performer == null)
                return NotFound();
            return Ok(performer);
        }

        [HttpPost]
        public IActionResult PerformerAdd([FromBody] Performer performer)
        {
            _repository.PerformerAdd(performer);
            return Ok();
        }

        [HttpPut]
        public IActionResult PerformerUpdate([FromBody] Performer performer)
        {
            _repository.PerformerUpdate(performer);
            return Ok();
        }

        [HttpDelete]
        public IActionResult PerformerDelete(int idPerformer)
        {
            _repository.PerformerDelete(idPerformer);
            return Ok();
        }
    }
}
