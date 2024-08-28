using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using LivingSpaces.Models;
using Microsoft.EntityFrameworkCore;
using LivingSpaces.ViewModels;

using System.Security.Claims;

namespace LivingSpaces.Controllers
{
    public class AccountsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountsController> _logger;
        private readonly ApplicationDbContext _context;

        public AccountsController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AccountsController> logger,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    IsDeleted = false
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    await _userManager.AddToRoleAsync(user, "User");

                    var freePlan = await _context.SubscriptionPlans
                        .FirstOrDefaultAsync(p => p.SubscriptionName == "Free");

                    if (freePlan != null)
                    {
                        var userSubscription = new UserSubscription
                        {
                            UserId = user.Id,
                            SubscriptionPlanId = freePlan.Id,
                            StartDate = DateTime.UtcNow,
                            EndDate = DateTime.MaxValue, 
                            IsActive = true,
                            RemainingListings = freePlan.NumberOfListings
                        };

                        _context.UserSubscriptions.Add(userSubscription);
                        await _context.SaveChangesAsync();
                    }

                    await _signInManager.SignInAsync(user, isPersistent: true);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }



        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);

                    if (user != null)
                    {
                        if (User.IsInRole("Admin"))
                        {
                            return RedirectToAction("Index", "Admin");
                        }
                        var userSubscription = await _context.UserSubscriptions
                            .Include(us => us.SubscriptionPlan)
                            .Where(us => us.UserId == user.Id)
                            .FirstOrDefaultAsync();

                        if (userSubscription != null)
                        {
                            var Plan = await _context.SubscriptionPlans
                                .FirstOrDefaultAsync(p => p.Id == userSubscription.SubscriptionPlanId);


                            if (userSubscription.EndDate < DateTime.Now || userSubscription.IsActive)
                            {
                                userSubscription.StartDate = DateTime.Now;
                                userSubscription.EndDate = userSubscription.StartDate.AddDays(30);
                                userSubscription.RemainingListings = Plan.NumberOfListings;

                                _context.Update(userSubscription);
                                await _context.SaveChangesAsync();

                            }

                        }
                    }

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction("Index", "Home");
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
