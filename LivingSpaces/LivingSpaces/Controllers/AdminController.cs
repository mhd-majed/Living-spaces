using LivingSpaces.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LivingSpaces.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var UsersCount = _context.Users.Count();

            var usersWithPaidSubscriptions = await _context.Users
                .Include(u => u.UserSubscription)
                .Where(u => u.UserSubscription != null && u.UserSubscription.SubscriptionPlanId > 1)
                .ToListAsync();
            var MontlyIncome = 0;
            var PaidUsersCount = 0;
            foreach (var u in usersWithPaidSubscriptions)
            {
                PaidUsersCount++;
                if (u.UserSubscription.SubscriptionPlanId == 1)
                {
                    MontlyIncome+=10;
                }
                else
                {
                    MontlyIncome += 15;
                }
            }
            var ReviewsCount = _context.UserReviews.Count();

            var messagesCount = _context.Texts.Count();

            var listingsCount = _context.Properties.Count();


            ViewBag.MontlyIncome = MontlyIncome;
            ViewBag.ReviewsCount = ReviewsCount;
            ViewBag.PaidUsersCount = PaidUsersCount;
            ViewBag.UsersCount = UsersCount;
            ViewBag.messagesCount = messagesCount;
            ViewBag.ListingsCount = listingsCount;

            return View();
        }


        // Action method to retrieve all users
        public async Task<IActionResult> Users(string searchTerm)
        {
            IQueryable<ApplicationUser> usersQuery = _userManager.Users;

            if (!string.IsNullOrEmpty(searchTerm))
            {
                usersQuery = usersQuery.Where(u => u.Email.Contains(searchTerm) || u.FirstName.Contains(searchTerm) || u.LastName.Contains(searchTerm));
            }

            IList<ApplicationUser> users = await usersQuery.ToListAsync();

            return View(users);
        }

        public async Task<IActionResult> PropertyListings(string searchTerm)
        {
            IQueryable<PropertyListing> propertiesQuery = _context.Properties
                .Include(p => p.ApplicationUser)
                .Include(p => p.PropertyCategory)
                .Include(p =>p.Address);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                propertiesQuery = propertiesQuery.Where(p =>
                    p.Title.Contains(searchTerm) ||
                    p.Description.Contains(searchTerm) ||
                    p.PropertyCategory.CategoryName.Contains(searchTerm));
            }

            var properties = await propertiesQuery.ToListAsync();

            return View(properties);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProperty([FromBody] int id)
        {
            Console.WriteLine("Received ID: " + id);

            var property = await _context.Properties.FindAsync(id);
            if (property != null)
            {
                property.IsDeleted = true;
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }



        [HttpPost]
        public async Task<IActionResult> DeleteUser([FromBody] string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user != null)
            {
                user.IsDeleted = true;
                await _userManager.UpdateAsync(user);
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        public async Task<IActionResult> Categories()
        {
            return View(await _context.Category.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory([FromBody] string id)
        {
            Console.WriteLine("hi bitch");
            var category = await _context.Category.FindAsync(id);

            if (category != null)
            {
                _context.Category.Remove(category);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        public IActionResult GetMonthlyUserCounts()
        {
            var users = _context.Users.ToList();

            var monthlyUserCounts = users
                .GroupBy(user => new
                {
                    Year = user.CreatedAt.Year,
                    Month = user.CreatedAt.Month
                })
                .Select(group => new
                {
                    MonthYear = $"{group.Key.Month}/{group.Key.Year}",
                    Count = group.Count()
                })
                .OrderBy(x => x.MonthYear)  
                .ToList();
            Console.WriteLine(Json(monthlyUserCounts));
            return Json(monthlyUserCounts);
        }

    }
}
