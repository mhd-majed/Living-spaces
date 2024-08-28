using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LivingSpaces.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Microsoft.IdentityModel.Tokens;
using System.Drawing.Printing;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using LivingSpaces.ViewModels;

namespace LivingSpaces.Controllers
{
    public class PropertyListingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PropertyListingsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // GET: PropertyListings
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Properties.Include(p => p.ApplicationUser).Include(p => p.PropertyCategory);
            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> Properties(string searchTerm, string listingType, List<string> selectedCategories,decimal? minPrice, decimal? maxPrice, int pageNumber = 1)
        {  
            
            ViewBag.ListingType = listingType;
            ViewBag.SelectedCategories = selectedCategories ?? new List<string>();
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.pageNumber = pageNumber;

            var applicationDbContext = _context.Properties
                .Include(p => p.ApplicationUser)
                .Include(p => p.PropertyCategory)
                .Include(p => p.ListingPhotos)
                .Include(p => p.Address)
                .OrderByDescending(p => p.ListedAt)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                applicationDbContext = applicationDbContext.Where(p =>
                    p.Title.Contains(searchTerm) ||
                    p.PropertyCategory.CategoryName.Contains(searchTerm) ||
                    p.Description.Contains(searchTerm) ||
                    p.Address.Street.Contains(searchTerm) ||
                    p.Address.City.Contains(searchTerm) ||
                    p.Address.State.Contains(searchTerm) ||
                    p.Address.Country.Contains(searchTerm));
            }

            if (!string.IsNullOrEmpty(listingType))
            {
                if (listingType =="For Sale")
                {
                    applicationDbContext = applicationDbContext.Where(p => p.PropertyType == PropertyType.Selling);
                }
                else
                {
                    applicationDbContext = applicationDbContext.Where(p => p.PropertyType == PropertyType.Renting);
                }
            }

            if (selectedCategories != null && selectedCategories.Any())
            {
                applicationDbContext = applicationDbContext.Where(p => selectedCategories.Contains(p.PropertyCategory.CategoryName));
            }

            if (minPrice.HasValue)
            {
                applicationDbContext = applicationDbContext.Where(p => p.Price >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                applicationDbContext = applicationDbContext.Where(p => p.Price <= maxPrice.Value);
            }

            var totalProperties = await applicationDbContext.CountAsync();
            var properties = await applicationDbContext
                .Skip((pageNumber - 1) * 6)
                .Take(6)
                .ToListAsync();

            var totalPages = (int)Math.Ceiling(totalProperties / (double)6);

            ViewBag.TotalPages = totalPages;

            return View(properties);
        }

        // GET: PropertyListings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var propertyListing = await _context.Properties
                .Include(p => p.ApplicationUser)
                .Include(p => p.PropertyCategory)
                .FirstOrDefaultAsync(m => m.PropertyListingID == id);
            if (propertyListing == null)
            {
                return NotFound();
            }

            return View(propertyListing);
        }

        // GET: PropertyListings/Create
        public async Task<IActionResult> Create()
        {

            var userId = _userManager.GetUserId(User);
            var user = await _context.Users
                .Include(u => u.UserSubscription)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null || user.UserSubscription == null)
            {
                return RedirectToAction("Error", "Home"); 
            }

            var subscriptionId = user.UserSubscription.SubscriptionPlanId;

            ViewData["UserID"] = userId; 
            ViewData["CategoryID"] = new SelectList(_context.Category, "CategoryID", "CategoryName");
            ViewData["PropertyType"] = new SelectList(Enum.GetValues(typeof(PropertyType)).Cast<PropertyType>());

            ViewData["SubscriptionID"] = subscriptionId;

            return View();
        }

        // POST: PropertyListings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PropertyListingID,Title,Description,Address,Price,Bedrooms,Bathrooms,SquareMeter,VRUrl,PropertyType,CategoryID")] PropertyListing propertyListing)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _context.Users
                .Include(u => u.UserSubscription)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return RedirectToAction("Error", "Home");
            }
            if (user.UserSubscription == null || user.UserSubscription.RemainingListings <= 0)
            {
                return RedirectToAction("Error", "Home"); 
            }

            if (string.IsNullOrWhiteSpace(propertyListing.Title) ||
                string.IsNullOrWhiteSpace(propertyListing.Description) ||
                propertyListing.Price == 0 ||
                propertyListing.Bedrooms == 0 ||
                propertyListing.Bathrooms == 0 ||
                propertyListing.SquareMeter == 0 ||
                propertyListing.CategoryID == 0 ) 
            {
                ViewData["UserID"] = userId;
                ViewData["CategoryID"] = new SelectList(_context.Category, "CategoryID", "CategoryName");
                ViewData["PropertyType"] = new SelectList(Enum.GetValues(typeof(PropertyType)).Cast<PropertyType>());
                ViewData["SubscriptionID"] = user.UserSubscription.SubscriptionPlanId;

                return View(propertyListing);
            }

            propertyListing.UserID = userId;
            propertyListing.EndDate = propertyListing.ListedAt.AddDays(user.UserSubscription.ListingActiveDays);

            _context.Add(propertyListing);
            await _context.SaveChangesAsync();

            user.UserSubscription.RemainingListings -= 1;
            _context.Update(user.UserSubscription);
            await _context.SaveChangesAsync();

            return RedirectToAction("UploadImages", "ListingPhotoes", new { propertyListingID = propertyListing.PropertyListingID });
        }

        public async Task<IActionResult> propertyDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var propertyListing = await _context.Properties
                .Include(p => p.ApplicationUser)
                    .ThenInclude(p => p.Statistics)
                .Include(p => p.PropertyCategory)
                .Include(p => p.ListingPhotos)
                .Include(p => p.Address)
                .Include(p => p.Booking) 
                    .ThenInclude(b => b.DailyBooking) 
                .Include(p => p.Booking)
                    .ThenInclude(b => b.WeeklyBooking)
                .FirstOrDefaultAsync(m => m.PropertyListingID == id);
            if (propertyListing == null)
            {
                return NotFound();
            }

            return View(propertyListing);
        }

        [HttpPost]
        public async Task<IActionResult> TourRequest([FromBody] TourViewModel requestModel)
        {
            var userId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(userId))
                {
                    TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("Jordan Standard Time");

                    DateTime localScheduledDate = TimeZoneInfo.ConvertTimeFromUtc(requestModel.ScheduledDate, timeZone);

                    DateTime localNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);

                    if (localScheduledDate < localNow)
                    {
                        return Json(new { success = false, message = "Invalid Input." });
                    }

                    var existingTour = await _context.Tours
                        .FirstOrDefaultAsync(t => t.PropertyListingID == requestModel.PropertyListingID
                                                && t.ScheduledDate == localScheduledDate);

                    if (existingTour != null)
                    {
                        return Json(new { success = false, message = "Already Booked." });
                    }

                    var tour = new Tour
                    {
                        PropertyListingID = requestModel.PropertyListingID,
                        UserID = userId,
                        ScheduledDate = localScheduledDate,
                        Status = "Received",
                        Comments = requestModel.Comments
                    };

                    _context.Tours.Add(tour);
                    await _context.SaveChangesAsync();

                    return Json(new { success = true });
                }

                return Json(new { success = false, message = "Invalid Input." });
            }

            return Json(new { success = false, message = "Invalid Input." });
        }

        // GET: PropertyListings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var propertyListing = await _context.Properties.FindAsync(id);
            if (propertyListing == null)
            {
                return NotFound();
            }

            var currentUserId = _userManager.GetUserId(User);

            if (propertyListing.UserID != currentUserId)
            {
                return Forbid();
            }

            ViewData["CategoryID"] = new SelectList(_context.Category, "CategoryID", "CategoryName", propertyListing.CategoryID);
            ViewData["PropertyType"] = new SelectList(Enum.GetValues(typeof(PropertyType)).Cast<PropertyType>());

            return View(propertyListing);
        }

        // POST: PropertyListings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PropertyListingID,Title,Description,Price,Bedrooms,Bathrooms,SquareMeter,VRUrl,PropertyType,CategoryID")] PropertyListing propertyListing)
        {
            if (id != propertyListing.PropertyListingID)
            {
                return NotFound();
            }

            try
            {
                var existingPropertyListing = await _context.Properties.AsNoTracking().FirstOrDefaultAsync(p => p.PropertyListingID == id);
                if (existingPropertyListing == null)
                {
                    return NotFound();
                }

                propertyListing.UserID = existingPropertyListing.UserID;

                _context.Update(propertyListing);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PropertyListingExists(propertyListing.PropertyListingID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction("UploadImages", "ListingPhotoes", new { propertyListingID = propertyListing.PropertyListingID });
        }

        // GET: PropertyListings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var propertyListing = await _context.Properties
                .Include(p => p.ApplicationUser)
                .Include(p => p.PropertyCategory)
                .FirstOrDefaultAsync(m => m.PropertyListingID == id);
            if (propertyListing == null)
            {
                return NotFound();
            }

            return View(propertyListing);
        }

        // POST: PropertyListings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var propertyListing = await _context.Properties.FindAsync(id);
            if (propertyListing != null)
            {
                _context.Properties.Remove(propertyListing);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PropertyListingExists(int id)
        {
            return _context.Properties.Any(e => e.PropertyListingID == id);
        }
    }
}
