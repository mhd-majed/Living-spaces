using LivingSpaces.Models;
using LivingSpaces.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LivingSpaces.Controllers
{
    public class BookingsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;


        public BookingsController(IConfiguration configuration, ApplicationDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public ActionResult CreateBooking(int propertyListingID)
        {
            var model = new BookingViewModel();
            ViewBag.PropertyListingID = propertyListingID;
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> CreateBooking(BookingViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var oldBooking = await _context.Bookings
                .FirstOrDefaultAsync(p => p.PropertyListingID == model.PropertyListingID);

            if (oldBooking != null)
            {
                _context.Bookings.Remove(oldBooking);
                await _context.SaveChangesAsync(); 
            }

            var booking = new Booking
            {
                BookingType = model.BookingType,
                PropertyListingID = model.PropertyListingID,
            };

            if (model.BookingType == BookingType.Daily)
            {
                if (model.DailyStartTime.HasValue && model.DailyEndTime.HasValue)
                {
                    booking.DailyBooking = new DailyBooking
                    {
                        StartTime = model.DailyStartTime.Value,
                        EndTime = model.DailyEndTime.Value
                    };
                }
                else
                {
                    ModelState.AddModelError("", "Daily booking requires both start and end times.");
                    return View(model);
                }
            }
            else if (model.BookingType == BookingType.Weekly)
            {
                if (model.MondayStartTime.HasValue && model.MondayEndTime.HasValue &&
                    model.TuesdayStartTime.HasValue && model.TuesdayEndTime.HasValue &&
                    model.WednesdayStartTime.HasValue && model.WednesdayEndTime.HasValue &&
                    model.ThursdayStartTime.HasValue && model.ThursdayEndTime.HasValue &&
                    model.FridayStartTime.HasValue && model.FridayEndTime.HasValue &&
                    model.SaturdayStartTime.HasValue && model.SaturdayEndTime.HasValue &&
                    model.SundayStartTime.HasValue && model.SundayEndTime.HasValue)
                {
                    booking.WeeklyBooking = new WeeklyBooking
                    {
                        MondayStartTime = model.MondayStartTime.Value,
                        MondayEndTime = model.MondayEndTime.Value,
                        TuesdayStartTime = model.TuesdayStartTime.Value,
                        TuesdayEndTime = model.TuesdayEndTime.Value,
                        WednesdayStartTime = model.WednesdayStartTime.Value,
                        WednesdayEndTime = model.WednesdayEndTime.Value,
                        ThursdayStartTime = model.ThursdayStartTime.Value,
                        ThursdayEndTime = model.ThursdayEndTime.Value,
                        FridayStartTime = model.FridayStartTime.Value,
                        FridayEndTime = model.FridayEndTime.Value,
                        SaturdayStartTime = model.SaturdayStartTime.Value,
                        SaturdayEndTime = model.SaturdayEndTime.Value,
                        SundayStartTime = model.SundayStartTime.Value,
                        SundayEndTime = model.SundayEndTime.Value
                    };
                }
                else
                {
                    ModelState.AddModelError("", "Weekly booking requires start and end times for all days of the week.");
                    return View(model);
                }
            }

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return RedirectToAction("SetLocation", "Maps", new { propertyListingID = model.PropertyListingID });
        }


    }
}
