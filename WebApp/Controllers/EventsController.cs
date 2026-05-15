using eventLib.Dal;
using eventLib.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApp.ViewModels;
using WebApp.Helpers;

namespace WebApp.Controllers
{
    [Authorize]
    public class EventsController : Controller
    {
        private readonly IApi _api;

        public EventsController(IApi api)
        {
            _api = api;
        }

        public async Task<ActionResult> Index(string? search = null)
        {
            var jwt = this.GetJwtToken();
            EventVM eventVM = new EventVM();
            eventVM.Events = await _api.MyEventsGet(jwt!, search);
            return View(eventVM);
        }

        public async Task<ActionResult> Details(int? idEvent)
        {
            var jwt = this.GetJwtToken();
            EventVM eventVM = new EventVM();
            eventVM.Event = (await _api.EventGet(jwt!, idEvent))!;
            eventVM.EventPerformers = await _api.EventPerformersGet(jwt!, idEvent);
            eventVM.EventTypes = await _api.EventTypesGet(jwt!);
            return View(eventVM);
        }

        public async Task<ActionResult> MyEventsDetails(int? idEvent)
        {
            var jwt = this.GetJwtToken();
            EventVM eventVM = new EventVM();
            eventVM.Event = (await _api.EventGet(jwt!, idEvent))!;
            eventVM.EventPerformers = await _api.EventPerformersGet(jwt!, idEvent);
            eventVM.EventTypes = await _api.EventTypesGet(jwt!);
            return View(eventVM);
        }

        public async Task<ActionResult> Add(int? idEvent)
        {
            var jwt = this.GetJwtToken();
            EventVM eventVM = new EventVM();
            eventVM.Event = (await _api.EventGet(jwt!, idEvent))!;
            eventVM.EventPerformers = await _api.EventPerformersGet(jwt!, idEvent);
            eventVM.EventTypes = await _api.EventTypesGet(jwt!);
            return View(eventVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegistrationConfirm(int? eventID)
        {
            try
            {
                var jwt = this.GetJwtToken();
                string? username = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
                User user = (await _api.UserGet(jwt, null, username))!;
                await _api.EventRegistrationAdd(jwt!, eventID, user.IDUser);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public async Task<ActionResult> MyEvents(string? search = null)
        {
            var jwt = this.GetJwtToken();
            EventVM eventVM = new EventVM();
            string? username = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            User user = (await _api.UserGet(jwt, null, username))!;
            eventVM.EventRegistrations = await _api.EventRegistrationsGet(jwt!, user.IDUser, search);
            return View(eventVM);
        }

        public async Task<ActionResult> Remove(int? idEvent)
        {
            var jwt = this.GetJwtToken();
            EventVM eventVM = new EventVM();
            eventVM.Event = (await _api.EventGet(jwt!, idEvent))!;
            eventVM.EventPerformers = await _api.EventPerformersGet(jwt!, idEvent);
            eventVM.EventTypes = await _api.EventTypesGet(jwt!);
            return View(eventVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegistrationRemoveConfirm(int? eventID)
        {
            try
            {
                var jwt = this.GetJwtToken();
                string? username = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
                User user = (await _api.UserGet(jwt, null, username))!;
                await _api.EventRegistrationDelete(jwt!, eventID, user.IDUser);
                return RedirectToAction(nameof(MyEvents));
            }
            catch
            {
                return View();
            }
        }
    }
}
