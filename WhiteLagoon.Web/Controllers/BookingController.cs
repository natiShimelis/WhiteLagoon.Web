using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WhiteLagoon.Application.Common.Utility;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;

namespace WhiteLagoon.Web.Controllers
{
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _db;

        public BookingController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult FinalizeBooking(int villaId, DateOnly checkInDate, int nights)
        {            
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ApplicationUser user = _db.Users.FirstOrDefault(u => u.Id == userId);


            Booking booking = new()
            {
                VillaId = villaId,
                Villa = _db.Villas.Include(v => v.VillaAmenity).FirstOrDefault(v => v.Id == villaId),
                CheckInDate = checkInDate,
                Nights = nights,
                CheckOutDate = checkInDate.AddDays(nights),
                UserId = userId,
                Phone = user.PhoneNumber,
                Email = user.Email,
                Name = user.Name
            };
            booking.TotalCost = booking.Villa.Price * nights;
            return View(booking);
        }


        [Authorize]
        [HttpPost]
        public IActionResult FinalizeBooking(Booking booking)
        {
            // Fetch the villa details based on the provided VillaId
            var villa = _db.Villas.FirstOrDefault(v => v.Id == booking.VillaId);

            // Ensure the villa exists before proceeding
            if (villa == null)
            {
                TempData["error"] = "Villa not found.";
                return RedirectToAction(nameof(FinalizeBooking), new
                {
                    villaId = booking.VillaId,
                    checkInDate = booking.CheckInDate,
                    nights = booking.Nights
                });
            }

            // Calculate the total cost of the booking based on the villa price and number of nights
            booking.TotalCost = villa.Price * booking.Nights;

            // Set the booking status to pending and store the current booking date
            booking.Status = SD.StatusPending;
            booking.BookingDate = DateTime.Now;

            // Check if the villa is available during the requested period
            bool isVillaAvailable = !_db.Bookings
                .Any(b => b.VillaId == villa.Id
                       && ((b.CheckInDate <= booking.CheckInDate && b.CheckOutDate > booking.CheckInDate) ||
                           (b.CheckInDate < booking.CheckOutDate && b.CheckOutDate >= booking.CheckOutDate)));

            if (!isVillaAvailable)
            {
                TempData["error"] = "Room has been sold out!";
                return RedirectToAction(nameof(FinalizeBooking), new
                {
                    villaId = booking.VillaId,
                    checkInDate = booking.CheckInDate,
                    nights = booking.Nights
                });
            }

            // Add the booking to the database
            _db.Bookings.Add(booking);
            _db.SaveChanges();

            // Redirect to a confirmation page with the booking details
            return RedirectToAction(nameof(BookingConfirmation), new { bookingId = booking.Id });
        }


        [Authorize]
        public IActionResult BookingConfirmation(int bookingId)
        {
            Booking bookingFromDb = _db.Bookings.Include(b => b.User).Include(b => b.Villa)
                .FirstOrDefault(b => b.Id == bookingId);
            if (bookingFromDb.Status == SD.StatusPending)
            {
                if(bookingFromDb.IsPaymentSuccessful)
                {
                    bookingFromDb.Status = SD.StatusApproved;
                    _db.SaveChanges();
                }
            }
                return View(bookingId);
        }
        [Authorize]
        [Authorize]
        public IActionResult BookingDetails(int bookingId)
        {
            var bookingFromDb = _db.Bookings.Include(b => b.Villa).FirstOrDefault(b => b.Id == bookingId);

            if (bookingFromDb == null)
            {
                return NotFound();
            }

            if (bookingFromDb.VillaNumber == 0 && bookingFromDb.Status == SD.StatusApproved)
            {
                // Get available villa numbers for the specified villa
                var availableVillaNumber = AssignAvailableVillaNumberByVilla(bookingFromDb.VillaId);

                // Retrieve available villa numbers and assign them to the booking
                bookingFromDb.VillaNumbers = _db.VillaNumbers
                    .Where(vn => vn.VillaId == bookingFromDb.VillaId && availableVillaNumber.Contains(vn.Villa_Number))
                    .ToList();
            }

            return View(bookingFromDb);
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult CheckIn(Booking booking)
        {
            var bookingFromDb = _db.Bookings.FirstOrDefault(b => b.Id == booking.Id);

            if (bookingFromDb == null)
            {
                return NotFound();
            }

            // Update the booking status to "Checked In" and set the villa number
            bookingFromDb.Status = SD.StatusCheckedIn;
            bookingFromDb.VillaNumber = booking.VillaNumber;

            _db.SaveChanges();

            TempData["Success"] = "Booking Updated Successfully.";
            return RedirectToAction(nameof(BookingDetails), new { bookingId = booking.Id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult CheckOut(Booking booking)
        {
            var bookingFromDb = _db.Bookings.FirstOrDefault(b => b.Id == booking.Id);

            if (bookingFromDb == null)
            {
                return NotFound();
            }

            // Update the booking status to "Completed"
            bookingFromDb.Status = SD.StatusCompleted;
            bookingFromDb.VillaNumber = booking.VillaNumber;

            _db.SaveChanges();

            TempData["Success"] = "Booking Completed Successfully.";
            return RedirectToAction(nameof(BookingDetails), new { bookingId = booking.Id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult CancelBooking(Booking booking)
        {
            var bookingFromDb = _db.Bookings.FirstOrDefault(b => b.Id == booking.Id);

            if (bookingFromDb == null)
            {
                return NotFound();
            }

            // Update the booking status to "Cancelled"
            bookingFromDb.Status = SD.StatusCancelled;
            bookingFromDb.VillaNumber = 0;

            _db.SaveChanges();

            TempData["Success"] = "Booking Cancelled Successfully.";
            return RedirectToAction(nameof(BookingDetails), new { bookingId = booking.Id });
        }


        private List<int> AssignAvailableVillaNumberByVilla(int villaId)
        {
            List<int> availableVillaNumbers = new();

            // Fetch all villa numbers for the specified villa
            var villaNumbers = _db.VillaNumbers.Where(vn => vn.VillaId == villaId).ToList();

            // Get a list of villa numbers that are currently checked in
            var checkedInVilla = _db.Bookings
                .Where(b => b.VillaId == villaId && b.Status == SD.StatusCheckedIn)
                .Select(b => b.VillaNumber)
                .ToList();

            // Add available villa numbers that are not currently checked in
            foreach (var villaNumber in villaNumbers)
            {
                if (!checkedInVilla.Contains(villaNumber.Villa_Number))
                {
                    availableVillaNumbers.Add(villaNumber.Villa_Number);
                }
            }

            return availableVillaNumbers;
        }


        #region API CALLS
        [HttpGet]
        [Authorize]
        public IActionResult GetAll(string status)
        {
            IEnumerable<Booking> objBookings;
            string userId = "";
            if (string.IsNullOrEmpty(status))
            {
                status = "";
            }

            if (!User.IsInRole(SD.Role_Admin))
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            }

            objBookings = _db.Bookings.Include(u => u.User).Include(v => v.Villa).Where(u => u.Status == status).ToList();

            return Json(new { data = objBookings });
        }

        #endregion
    }
}
