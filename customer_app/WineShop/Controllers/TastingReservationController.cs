using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WineShop.Domain.DomainModels;
using WineShop.Domain.DTO;
using WineShop.Domain.EMail;
using WineShop.Service.Interface;
using WineShop.Services.Interface;

namespace WineShop.Web.Controllers
{
    [Authorize]
    public class TastingReservationController : Controller
    {
        private readonly ITastingReservationService _tastingReservationService;
        private readonly IEmailService _emailService;

        public TastingReservationController(
            ITastingReservationService tastingReservationService,
            IEmailService emailService)
        {
            _tastingReservationService = tastingReservationService;
            _emailService = emailService;
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
        public async Task<IActionResult> Book(TastingReservationCreateViewModel model)
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

            string reservationTime = model.BlocksWholeDay
                ? "Целодневна резервација"
                : model.SelectedTime;

            var userMail = new EmailMessage
            {
                MailTo = model.Email,
                Subject = "Потврда за резервација за дегустација - Винарија Кувин",
                Content = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #e5e5e5; border-radius: 10px;'>
                    <h2 style='color: #7b1e3a; margin-bottom: 20px;'>Вашата резервација е успешно креирана</h2>

                    <p style='font-size: 15px; color: #333;'>
                        Ви благодариме за интересот и успешно резервиравте термин за дегустација во <b>Винарија Кувин</b>.
                    </p>

                    <p style='font-size: 15px; color: #333;'>
                        Во продолжение се прикажани деталите за Вашата резервација:
                    </p>

                    <hr style='margin: 20px 0; border: none; border-top: 1px solid #ddd;' />

                    <p style='font-size: 15px; color: #333;'><b>Име и презиме:</b> {model.FullName}</p>
                    <p style='font-size: 15px; color: #333;'><b>Пакет:</b> {model.PackageName}</p>
                    <p style='font-size: 15px; color: #333;'><b>Датум:</b> {model.Date:dd.MM.yyyy}</p>
                    <p style='font-size: 15px; color: #333;'><b>Време:</b> {reservationTime}</p>
                    <p style='font-size: 15px; color: #333;'><b>Број на гости:</b> {model.NumberOfGuests}</p>
                    <p style='font-size: 15px; color: #333;'><b>Телефон:</b> {model.PhoneNumber}</p>
                    <p style='font-size: 15px; color: #333;'><b>Времетраење:</b> {model.DurationHours} часа</p>
                    <p style='font-size: 15px; color: #333;'><b>Цена:</b> {model.Price} МКД</p>

                    <hr style='margin: 20px 0; border: none; border-top: 1px solid #ddd;' />

                    <p style='font-size: 14px; color: #666;'>
                        Ве очекуваме во избраниот термин. Доколку имате потреба од дополнителни информации, ве молиме контактирајте нè навремено.
                    </p>

                    <p style='font-size: 13px; color: #888; margin-top: 20px;'>
                        Ви благодариме на довербата.<br />
                        Со почит,<br />
                        Винарија Кувин
                    </p>
                </div>",
                status = true
            };

            var adminMail = new EmailMessage
            {
                MailTo = "mia.stevkovska@yahoo.com",
                Subject = "Нова резервација за дегустација - Винарија Кувин",
                Content = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #e5e5e5; border-radius: 10px;'>
                    <h2 style='color: #7b1e3a; margin-bottom: 20px;'>Нова резервација за дегустација</h2>

                    <p style='font-size: 15px; color: #333;'><b>Име и презиме:</b> {model.FullName}</p>
                    <p style='font-size: 15px; color: #333;'><b>Email:</b> {model.Email}</p>
                    <p style='font-size: 15px; color: #333;'><b>Телефон:</b> {model.PhoneNumber}</p>
                    <p style='font-size: 15px; color: #333;'><b>Пакет:</b> {model.PackageName}</p>
                    <p style='font-size: 15px; color: #333;'><b>Датум:</b> {model.Date:dd.MM.yyyy}</p>
                    <p style='font-size: 15px; color: #333;'><b>Време:</b> {reservationTime}</p>
                    <p style='font-size: 15px; color: #333;'><b>Број на гости:</b> {model.NumberOfGuests}</p>
                    <p style='font-size: 15px; color: #333;'><b>Цена:</b> {model.Price} МКД</p>

                    <p style='font-size: 14px; color: #666; margin-top: 20px;'>
                        Резервацијата е успешно внесена и може да се прегледа во административниот панел.
                    </p>
                </div>",
                status = true
            };

            await _emailService.SendEmailAsync(userMail);
            await _emailService.SendEmailAsync(adminMail);

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