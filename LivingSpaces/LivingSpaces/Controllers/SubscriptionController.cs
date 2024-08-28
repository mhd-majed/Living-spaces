using LivingSpaces.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LivingSpaces.Controllers
{
    public class SubscriptionController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public SubscriptionController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Checkout(int id)
        {
            var subscriptionPlan = _context.SubscriptionPlans.FirstOrDefault(sp => sp.Id == id);

            if (subscriptionPlan == null)
            {
                return NotFound();
            }

            ViewBag.Price = subscriptionPlan.Price;
            ViewBag.Id = subscriptionPlan.Id;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(PaymentViewModel model)
        {

            var subscriptionPlan = _context.SubscriptionPlans.FirstOrDefault(sp => sp.Id == model.SubscriptionPlanId);

            if (ModelState.IsValid)
            {

                if (subscriptionPlan != null )
                {
                    var user = await _userManager.GetUserAsync(User);

                    if (user != null)
                    {
                        var userSubscription = await _context.UserSubscriptions
                            .FirstOrDefaultAsync(us => us.UserId == user.Id);

                        if (userSubscription != null)
                        {
                            userSubscription.SubscriptionPlanId = subscriptionPlan.Id;
                            userSubscription.StartDate = DateTime.UtcNow;
                            userSubscription.EndDate = DateTime.UtcNow.AddMonths(1);
                            userSubscription.IsActive = true;
                            userSubscription.RemainingListings = subscriptionPlan.NumberOfListings;
                            userSubscription.ListingActiveDays = subscriptionPlan.ListingActiveDays;

                        }
                        else
                        {
                            userSubscription = new UserSubscription
                            {
                                UserId = user.Id,
                                SubscriptionPlanId = subscriptionPlan.Id,
                                StartDate = DateTime.UtcNow,
                                EndDate = DateTime.UtcNow.AddMonths(1),
                                IsActive = true,
                                RemainingListings = subscriptionPlan.NumberOfListings
                            };
                            _context.UserSubscriptions.Add(userSubscription);
                        }

                        await _context.SaveChangesAsync();

                        user.UserSubscription = userSubscription;
                        await _userManager.UpdateAsync(user);

                        return RedirectToAction("Confirmation");
                    }
                }
            }


            if (subscriptionPlan != null)
            {
                ViewBag.Price = subscriptionPlan.Price;
                ViewBag.Id = subscriptionPlan.Id;

            }
            return View(model);
        }
        public IActionResult Confirmation()
        {
            return View(); 
        }

    }
}
