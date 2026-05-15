using eventLib.Dal;
using eventLib.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.ViewModels;
using WebApp.Helpers;

namespace WebApp.Controllers
{
    public class PerformerManagementController : Controller
    {
        private readonly IApi _api;

        public PerformerManagementController(IApi api)
        {
            _api = api;
        }

        [Authorize]
        public async Task<ActionResult> Index(string? search = null)
        {
            var jwt = this.GetJwtToken();
            PerformerVM performerVM = new PerformerVM();
            performerVM.Performers = await _api.PerformersGet(jwt!, search);
            return View(performerVM);
        }

        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(PerformerVM performerVM)
        {
            try
            {
                var jwt = this.GetJwtToken();
                await _api.PerformerAdd(jwt!, performerVM.Performer);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        [Authorize]
        public async Task<ActionResult> Edit(int idPerformer)
        {
            var jwt = this.GetJwtToken();
            PerformerVM performerVM = new PerformerVM();
            performerVM.Performer = (await _api.PerformerGet(jwt!, idPerformer))!;
            return View(performerVM);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(PerformerVM value)
        {
            try
            {
                var jwt = this.GetJwtToken();
                await _api.PerformerUpdate(jwt!, value.Performer);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        [Authorize]
        public async Task<ActionResult> Delete(int? idPerformer)
        {
            var jwt = this.GetJwtToken();
            Performer value = (await _api.PerformerGet(jwt!, idPerformer))!;
            return View(value);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirm(int idPerformer)
        {
            try
            {
                var jwt = this.GetJwtToken();
                await _api.PerformerDelete(jwt!, idPerformer);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}
