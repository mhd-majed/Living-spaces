using LivingSpaces.Models;
using LivingSpaces.Models.LivingSpaces.Models;
using LivingSpaces.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace LivingSpaces.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReviewsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> UserReviews(string id)
        {

            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Statistics) 
                .FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            var userReviews = _context.UserReviews
                                       .Where(r => r.ReviewedUserID == id && !r.IsDeleted)
                                       .OrderByDescending(r => r.Rating)
                                       .ToList();

            var model = new UserReviewViewModel
            {
                ApplicationUser = user,
                UserReviews = userReviews
            };

            return View(model);
        }
    
    
        [HttpPost]
        public async Task<IActionResult> SubmitReview(SubmitReviewViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);

                var reviewedUser = await _context.Users.FindAsync(model.ReviewedUserID);

                if (userId == null || reviewedUser == null)
                {
                    return NotFound();
                }

                var review = new UserReview
                {
                    Rating = model.Rating,
                    ReviewText = model.ReviewText,
                    ReviewerID = userId,
                    ReviewedUserID = model.ReviewedUserID,
                    CreatedAt = DateTime.Now
                };
                _context.UserReviews.Add(review);

                var userStatistics = await _context.UserRatingStatistics
                                     .FirstOrDefaultAsync(s => s.UserID == model.ReviewedUserID);

                if (userStatistics == null)
                {
                    userStatistics = new UserRatingStatistics
                    {
                        UserID = model.ReviewedUserID,
                        RatingCount = 1,
                        AverageRating = model.Rating
                    };
                    _context.UserRatingStatistics.Add(userStatistics);
                }
                else
                {
                    userStatistics.RatingCount++;
                    userStatistics.AverageRating = ((userStatistics.AverageRating * (userStatistics.RatingCount - 1)) + model.Rating) / userStatistics.RatingCount;
                }


                await _context.SaveChangesAsync();

                return RedirectToAction("UserReviews", "Reviews", new { id = model.ReviewedUserID });
            }

            return View(model);
        }

        public async Task<IActionResult> UserProperties(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Statistics)
                .FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            var properties = await _context.Properties
                .Include(p => p.ApplicationUser)
                .Include(p => p.PropertyCategory)
                .Include(p => p.ListingPhotos)
                .Include(p =>p.Address)
                .Where(p => p.UserID == user.Id)
                .ToListAsync();

            var viewModel = new UserPropertiesViewModel
            {
                ApplicationUser = user,
                Properties = properties
            };

            return View(viewModel);
        }

    }
}
