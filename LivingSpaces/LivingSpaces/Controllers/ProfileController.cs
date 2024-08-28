using LivingSpaces.Models;
using LivingSpaces.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LivingSpaces.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public ProfileController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public async Task<IActionResult> MyProperties()
        {
            var user = await _context.Users
                .Include(u => u.UserSubscription) 
                .FirstOrDefaultAsync(u => u.Id == _userManager.GetUserId(User));

            if (user == null)
            {
                return RedirectToAction("Error", "Home");
            }

            int numberOfListings = user.UserSubscription.RemainingListings;

            var applicationDbContext = _context.Properties
                .Include(p => p.ApplicationUser)
                .Include(p => p.PropertyCategory)
                .Include(p => p.ListingPhotos)
                .Include(p => p.Address)
                .Where(p => p.UserID ==user.Id);

            ViewBag.NumberOfListings = numberOfListings;

            return View(await applicationDbContext.ToListAsync());
        }


        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }
            var profileImageUrl = string.IsNullOrEmpty(user.ProfileImageUrl)
                ? Url.Content("~/assets/images/user.jpg")
                : user.ProfileImageUrl;

            var viewModel = new ProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                ProfileImageUrl = profileImageUrl
            };
            Console.WriteLine($"ProfileImageUrl: {viewModel.ProfileImageUrl}");

            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> UpdateUserProfile(ProfileViewModel model, IFormFile ProfileImage)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;
                user.PhoneNumber = model.PhoneNumber;

                if (ProfileImage != null && ProfileImage.Length > 0)
                {
                    var extension = Path.GetExtension(ProfileImage.FileName);

                    var fileName = $"{Guid.NewGuid()}{extension}";

                    var filePath = Path.Combine("wwwroot/profileIamges", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ProfileImage.CopyToAsync(stream);
                    }

                    user.ProfileImageUrl = $"/profileIamges/{fileName}";
                }

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Your profile has been updated successfully.";
                    return RedirectToAction("Profile");
                }
            }


            model.ProfileImageUrl = user.ProfileImageUrl;

            return View("Profile", model);
        }



        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Profile", model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Your password has been changed successfully.";
                return RedirectToAction("Profile");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View("Profile", model);
        }

        [HttpGet]
        public async Task<IActionResult> Subscription()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var subscription = await _context.UserSubscriptions
                .Include(us => us.SubscriptionPlan)
                .Where(us => us.UserId == userId)
                .FirstOrDefaultAsync();

            return View(subscription);
        }

        [HttpGet]
        public async Task<IActionResult> TourRequests(int id)
        {

            var tourDates = await _context.Tours
                .Include(t => t.User)
                .Where(t => t.PropertyListingID == id)
                .ToListAsync();

            if (tourDates != null)
            {
                foreach (var tourDate in tourDates)
                {
                    Console.WriteLine(tourDate.Comments);
                }
            }
            return View(tourDates);
        }
        [HttpGet]
        public async Task<IActionResult> MyTours()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var tourDates = await _context.Tours
                .Include(t => t.User)
                .Where(t => t.UserID == userId)
                .ToListAsync();

            if (tourDates != null)
            {
                foreach (var tourDate in tourDates)
                {
                    Console.WriteLine(tourDate.Comments);
                }
            }
            return View(tourDates);
        }


        [HttpPost]
        public async Task<IActionResult> ConfirmTour(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var tour = await _context.Tours
                .Include(t => t.PropertyListing) 
                .FirstOrDefaultAsync(t => t.TourID == id);

            if (tour == null)
            {
                return NotFound();
            }

            if (tour.PropertyListing.UserID != userId)
            {
                return Unauthorized(); 
            }

            tour.Status = "Confirmed";
            _context.Update(tour);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Tour confirmed successfully." });
        }

        [HttpPost]
        public async Task<IActionResult> CancelTour(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var tour = await _context.Tours
                .Include(t => t.PropertyListing)
                .FirstOrDefaultAsync(t => t.TourID == id);

            if (tour == null)
            {
                return NotFound();
            }

            if (tour.PropertyListing.UserID != userId)
            {
                return Unauthorized();
            }

            tour.Status = "Cancelled";
            _context.Update(tour);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Tour cancelled successfully." });
        }
        [HttpPost]
        public async Task<IActionResult> CancelMyTour(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var tour = await _context.Tours
                .FirstOrDefaultAsync(t => t.TourID == id);

            if (tour == null)
            {
                return NotFound();
            }

            if (tour.UserID != userId)
            {
                return Unauthorized();
            }

            tour.Status = "Cancelled";
            _context.Update(tour);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Tour cancelled successfully." });
        }

    }
}
