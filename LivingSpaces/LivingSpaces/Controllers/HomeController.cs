using LivingSpaces.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace LivingSpaces.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Admin");
            }
            var subscriptionPlans = await _context.SubscriptionPlans.ToListAsync();

            var propertyListings = await _context.Properties
                .Include(p => p.ListingPhotos)
                .Include(p => p.PropertyCategory)
                .Include(p => p .Address)
                .OrderByDescending(p => p.ListedAt)
                .Take(6)
                .ToListAsync();

            var viewModel = new
            {
                SubscriptionPlans = subscriptionPlans,
                PropertyListings = propertyListings
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
