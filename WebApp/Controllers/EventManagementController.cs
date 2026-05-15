using eventLib.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.ViewModels;
using WebApp.Helpers;
using eventLib.Dal.API;

namespace WebApp.Controllers
{
    public class EventManagementController : Controller
    {
        private readonly IApi _api;

        public EventManagementController(IApi api)
        {
            _api = api;
        }

        [Authorize]
        public async Task<ActionResult> Index(string? search = null)
        {
            var jwt = this.GetJwtToken();
            EventVM eventVM = new EventVM();
            eventVM.Events = await _api.EventsGet(jwt!, search);
            return View(eventVM);
        }

        [Authorize]
        public async Task<ActionResult> Details(int idEvent)
        {
            var jwt = this.GetJwtToken();
            EventVM eventVM = new EventVM();
            eventVM.Event = (await _api.EventGet(jwt!, idEvent))!;
            eventVM.EventPerformers = await _api.EventPerformersGet(jwt!, idEvent);
            eventVM.EventTypes = await _api.EventTypesGet(jwt!);
            return View(eventVM);
        }

        [Authorize]
        public async Task<ActionResult> Create()
        {
            try
            {
                var jwt = this.GetJwtToken();
                var errMsg = TempData["ErrorMessage"] as string;
                ViewBag.ErrorMessage = errMsg;
                EventVM eventVM = new EventVM();
                eventVM.EventTypes = await _api.EventTypesGet(jwt!);
                return View(eventVM);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(EventVM eventVM, IFormFile imageFile)
        {
            try
            {
                var jwt = this.GetJwtToken();
                if (imageFile != null)
                {
                    var memoryStream = new MemoryStream();
                    await imageFile.CopyToAsync(memoryStream);
                    eventVM.Event.ImageName = imageFile.FileName;
                    eventVM.Event.ImageData = memoryStream.ToArray();
                }

                // ensure EventTypeName is populated so API model validation passes
                try
                {
                    var types = await _api.EventTypesGet(jwt!);
                    var sel = types.FirstOrDefault(t => t.IDEventType == eventVM.Event.eventTypeID);
                    if (sel != null)
                        eventVM.Event.EventTypeName = sel.EventTypeName;
                }
                catch { }

                await _api.EventAdd(jwt!, eventVM.Event);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                var jwt = this.GetJwtToken();
                var vm = eventVM ?? new EventVM();
                try
                {
                    vm.EventTypes = await _api.EventTypesGet(jwt!);
                }
                catch { }
                ModelState.AddModelError(string.Empty, e.Message);
                return View(vm);
            }
        }

        [Authorize]
        public async Task<ActionResult> Edit(int idEvent)
        {
            var jwt = this.GetJwtToken();
            EventVM eventVM = new EventVM();
            eventVM.Event = (await _api.EventGet(jwt!, idEvent))!;
            eventVM.EventPerformers = await _api.EventPerformersGet(jwt!, idEvent);
            eventVM.EventTypes = await _api.EventTypesGet(jwt!);
            return View(eventVM);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EventVM eventVM, IFormFile? imageFile)
        {
            try
            {
                var jwt = this.GetJwtToken();
                if (imageFile != null)
                {
                    var memoryStream = new MemoryStream();
                    await imageFile.CopyToAsync(memoryStream);
                    eventVM.Event.ImageName = imageFile.FileName;
                    eventVM.Event.ImageData = memoryStream.ToArray();
                }

                // ensure EventTypeName is populated so API model validation passes
                try
                {
                    var types = await _api.EventTypesGet(jwt!);
                    var sel = types.FirstOrDefault(t => t.IDEventType == eventVM.Event.eventTypeID);
                    if (sel != null)
                        eventVM.Event.EventTypeName = sel.EventTypeName;
                }
                catch { }

                await _api.EventUpdate(jwt!, eventVM.Event);
                return RedirectToAction("Edit", new { idEvent = eventVM.Event.IDEvent });
            }
            catch (Exception e)
            {
                var jwt = this.GetJwtToken();
                if (eventVM == null)
                {
                    eventVM = new EventVM();
                }
                try
                {
                    eventVM.EventTypes = await _api.EventTypesGet(jwt!);
                    if (eventVM.Event?.IDEvent != null)
                    {
                        eventVM.EventPerformers = await _api.EventPerformersGet(jwt!, eventVM.Event.IDEvent);
                    }
                }
                catch { }
                ModelState.AddModelError(string.Empty, e.Message);
                return View(eventVM);
            }
        }

        [Authorize]
        public async Task<ActionResult> Delete(int? idEvent)
        {
            var jwt = this.GetJwtToken();
            Event value = (await _api.EventGet(jwt!, idEvent))!;
            return View(value);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirm(int idEvent)
        {
            try
            {
                var jwt = this.GetJwtToken();
                await _api.EventDelete(jwt!, idEvent);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [Authorize]
        public async Task<ActionResult> EventPerformerDelete(int? eventID, int? performerID)
        {
            var jwt = this.GetJwtToken();
            await _api.EventPerformerDelete(jwt!, eventID, performerID);
            return RedirectToAction("Edit", new { idEvent = eventID });
        }

        [Authorize]
        public async Task<ActionResult> EventPerformerAdd(int? eventID, string? search = null)
        {
            var jwt = this.GetJwtToken();
            EventVM eventVM = new EventVM();
            eventVM.Event.IDEvent = eventID;
            eventVM.Performers = await _api.PerformersGet(jwt!, search);
            return View(eventVM);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EventPerformerAdd(EventVM value)
        {
            try
            {
                var jwt = this.GetJwtToken();
                await _api.EventPerformerAdd(jwt!, value.Event.IDEvent, value.EventPerformer.PerformerID);
                return RedirectToAction("Edit", new { idEvent = value.Event.IDEvent });
            }
            catch
            {
                return View();
            }
        }
    }
}
