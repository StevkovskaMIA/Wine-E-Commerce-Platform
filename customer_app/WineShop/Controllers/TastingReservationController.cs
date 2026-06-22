using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WineShop.Domain.DTO;
using WineShop.Service.Interface;

namespace WineShop.Web.Controllers
{
    [Authorize]
    public class TastingReservationController : Controller
    {
        private readonly ITastingReservationService _tastingReservationService;

        public TastingReservationController(ITastingReservationService tastingReservationService)
        {
            _tastingReservationService = tastingReservationService;
        }
        
            public IActionResult Index()
            {
                return View();
            }
        

        [HttpGet]
        public IActionResult Book(Guid packageId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var model = _tastingReservationService.GetBookingModel(packageId, userId);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Book(TastingReservationCreateViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var freshModel = _tastingReservationService.GetBookingModel(model.PackageId, userId);
            if (freshModel == null)
            {
                return NotFound();
            }

            model.PackageName = freshModel.PackageName;
            model.PackageDescription = freshModel.PackageDescription;
            model.DurationHours = freshModel.DurationHours;
            model.MaxGuests = freshModel.MaxGuests;
            model.Price = freshModel.Price;
            model.BlocksWholeDay = freshModel.BlocksWholeDay;
            model.AvailableTimes = _tastingReservationService.GetAvailableTimeSlots(model.PackageId, model.Date);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string message;
            var result = _tastingReservationService.CreateReservation(model, userId, out message);

            if (!result)
            {
                ModelState.AddModelError("", message);
                return View(model);
            }

            TempData["SuccessMessage"] = message;
            return RedirectToAction(nameof(Confirmation));
        }

        [HttpGet]
        public IActionResult GetAvailableSlots(Guid packageId, DateTime date)
        {
            var slots = _tastingReservationService.GetAvailableTimeSlots(packageId, date);
            return Json(slots);
        }

        [HttpGet]
        public IActionResult Confirmation()
        {
            return View();
        }
        public IActionResult MyReservations()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var reservations = _tastingReservationService.GetReservationsForUser(userId);
            return View(reservations);
        }

        public IActionResult Cancel(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var result = _tastingReservationService.CancelReservation(id, userId);

            if (result)
            {
                TempData["SuccessMessage"] = "Резервацијата е успешно откажана.";
            }
            else
            {
                TempData["ErrorMessage"] = "Откажување не е можно помалку од 24 часа пред дегустацијата. За помош, контактирајте не на +389 75 233 340.";
            }

            return RedirectToAction(nameof(MyReservations));
        }
    }
}