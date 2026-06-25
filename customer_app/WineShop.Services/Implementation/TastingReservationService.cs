using Microsoft.EntityFrameworkCore;
using WineShop.Domain.DomainModels;
using WineShop.Domain.DTO;
using WineShop.Repository;
using WineShop.Repository.Interface;
using WineShop.Service.Interface;

namespace WineShop.Service.Implementation
{
    public class TastingReservationService : ITastingReservationService
    {
        private readonly ApplicationDbContext _context;
        private readonly ITastingReservationRepository _tastingReservationRepository;
        private readonly IRepository<EmailMessage> _mailRepository;



        public TastingReservationService(
            ApplicationDbContext context,
            ITastingReservationRepository tastingReservationRepository, IRepository<EmailMessage> mailRepository)
        {
            _context = context;
            _tastingReservationRepository = tastingReservationRepository;
            _mailRepository = mailRepository;
        }

        public TastingReservationCreateViewModel? GetBookingModel(Guid packageId, string userId)
        {
            var packageItem = _context.TastingPackages.FirstOrDefault(x => x.Id == packageId);

            if (packageItem == null)
            {
                return null;
            }

            var user = _context.Users.FirstOrDefault(x => x.Id == userId);

            var model = new TastingReservationCreateViewModel
            {
                PackageId = packageItem.Id,
                PackageName = packageItem.Name,
                PackageDescription = packageItem.Description,
                DurationHours = packageItem.DurationHours,
                MaxGuests = packageItem.MaxGuests,
                Price = packageItem.Price,
                BlocksWholeDay = packageItem.BlocksWholeDay,
                Date = DateTime.Today,
                NumberOfGuests = 1,
                FullName = user != null ? (user.FirstName + " " + user.LastName).Trim() : "",
                Email = user?.Email ?? "",
                PhoneNumber = user?.PhoneNumber
            };

            if (!packageItem.BlocksWholeDay)
            {
                model.AvailableTimes = GetAvailableTimeSlots(packageId, DateTime.Today);
            }

            return model;
        }

        public List<string> GetAvailableTimeSlots(Guid packageId, DateTime date)
        {
            var packageItem = _context.TastingPackages.FirstOrDefault(x => x.Id == packageId);

            if (packageItem == null)
            {
                return new List<string>();
            }

            var reservationsForDay = _tastingReservationRepository.GetReservationsForDate(date);

            if (packageItem.BlocksWholeDay)
            {
                if (reservationsForDay.Any())
                {
                    return new List<string>();
                }

                return new List<string> { "Whole day" };
            }

            if (reservationsForDay.Any(x => x.TastingPackage.BlocksWholeDay && x.Status == "Confirmed"))
            {
                return new List<string>();
            }

            var result = new List<string>();

            int openHour = 10;
            int closeHour = 20;

            for (int hour = openHour; hour <= closeHour - packageItem.DurationHours; hour++)
            {
                var slotStart = date.Date.AddHours(hour);
                var slotEnd = slotStart.AddHours(packageItem.DurationHours);

                if (slotStart < DateTime.Now)
                {
                    continue;
                }

                var overlappingReservations = _tastingReservationRepository
                    .GetOverlappingReservations(slotStart, slotEnd)
                    .Where(x => x.Status == "Confirmed")
                    .ToList();

                if (!overlappingReservations.Any())
                {
                    result.Add(slotStart.ToString("HH:mm"));
                }
            }

            return result;
        }

        public bool CreateReservation(TastingReservationCreateViewModel model, string userId, out string message)
        {
            var packageItem = _context.TastingPackages.FirstOrDefault(x => x.Id == model.PackageId);

            if (packageItem == null)
            {
                message = "Package not found.";
                return false;
            }

            if (string.IsNullOrEmpty(userId))
            {
                message = "You must be logged in.";
                return false;
            }

            if (model.NumberOfGuests < 1 || model.NumberOfGuests > packageItem.MaxGuests)
            {
                message = $"Maximum number of guests is {packageItem.MaxGuests}.";
                return false;
            }

            DateTime reservationStart;
            DateTime reservationEnd;

            if (packageItem.BlocksWholeDay)
            {
                reservationStart = model.Date.Date;
                reservationEnd = model.Date.Date.AddDays(1);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(model.SelectedTime))
                {
                    message = "Please select a time.";
                    return false;
                }

                if (!TimeSpan.TryParse(model.SelectedTime, out var selectedTime))
                {
                    message = "Invalid selected time.";
                    return false;
                }

                reservationStart = model.Date.Date.Add(selectedTime);
                reservationEnd = reservationStart.AddHours(packageItem.DurationHours);
            }

            if (reservationStart < DateTime.Now)
            {
                message = "You cannot reserve a past time.";
                return false;
            }

            var overlappingReservations = _tastingReservationRepository
                .GetOverlappingReservations(reservationStart, reservationEnd)
                .Where(x => x.Status == "Confirmed")
                .ToList();

            if (overlappingReservations.Any())
            {
                if (packageItem.BlocksWholeDay)
                {
                    message = "This day is already reserved.";
                }
                else
                {
                    message = "This time slot is not available.";
                }

                return false;
            }

            var reservation = new TastingReservation
            {
                Id = Guid.NewGuid(),
                FullName = model.FullName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                ReservationStart = reservationStart,
                ReservationEnd = reservationEnd,
                NumberOfGuests = model.NumberOfGuests,
                TastingPackageId = model.PackageId,
                Status = "Confirmed",
                CreatedAt = DateTime.UtcNow,
                UserId = userId
            };

            _tastingReservationRepository.Insert(reservation);

            message = "Reservation created successfully.";
            return true;
        }

        public List<TastingReservation> GetReservationsForUser(string userId)
        {
            return _tastingReservationRepository.GetReservationsForUser(userId);
        }

        public bool CancelReservation(Guid reservationId, string userId)
        {
            var reservation = _tastingReservationRepository.GetByIdForUser(reservationId, userId);

            if (reservation == null)
                return false;

            if (reservation.Status == "Cancelled")
                return false;

            if ((reservation.ReservationStart - DateTime.Now).TotalHours < 24)
                return false;

            reservation.Status = "Cancelled";
            _tastingReservationRepository.Update(reservation);

            var userMessage = new EmailMessage
            {
                Id = Guid.NewGuid(),
                MailTo = reservation.Email,
                Subject = "Откажана резервација за дегустација",
                Content = $"Вашата резервација за {reservation.ReservationStart:dd.MM.yyyy HH:mm} е успешно откажана.",
                status = false
            };

            var adminMessage = new EmailMessage
            {
                Id = Guid.NewGuid(),
                MailTo = "mia.stevkovska@yahoo.com",
                Subject = "Откажана резервација за дегустација",
                Content = $"Резервацијата на {reservation.FullName} за {reservation.ReservationStart:dd.MM.yyyy HH:mm} е откажана.",
                status = false
            };

            _mailRepository.Insert(userMessage);
            _mailRepository.Insert(adminMessage);

            return true;
        }
    }
}