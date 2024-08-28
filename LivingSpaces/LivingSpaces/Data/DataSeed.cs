using Microsoft.AspNetCore.Identity;
using LivingSpaces.Models;
namespace LivingSpaces.Data
{
    public static class DataSeed
    {
        public static async Task SeedRolesAndAdminUserAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            string[] roleNames = { "Admin", "User","seller" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Seed subscription tiers
            if (!context.SubscriptionPlans.Any())
            {
                context.SubscriptionPlans.Add(new SubscriptionPlan
                {
                    SubscriptionName = "Free",
                    Price = 0M,
                    Description = "Ideal for individuals looking to list a single property without incurring any costs. Perfect for homeowners or first-time sellers seeking a hassle-free way to get started.",
                    NumberOfListings = 1,
                    ListingActiveDays = 30,
                    SupportVr = "Does not support virtual reality tours"
                });

                context.SubscriptionPlans.Add(new SubscriptionPlan
                {
                    SubscriptionName = "Pro",
                    Price = 19.99M,
                    Description = "Designed for individuals or agents who manage multiple properties and seek more visibility. This plan offers enhanced features to help you reach more potential buyers.",
                    NumberOfListings = 5,
                    ListingActiveDays = 60,
                    SupportVr = "Does support virtual reality tours"

                });
                context.SubscriptionPlans.Add(new SubscriptionPlan
                {
                    SubscriptionName = "Elite",
                    Price = 49.99M,
                    Description = "Tailored for real estate companies or agencies with a large number of properties. This plan provides maximum exposure and advanced tools to manage your listings effectively.",
                    NumberOfListings = 10,
                    ListingActiveDays = 90,
                    SupportVr = "Does support virtual reality tours"

                });
                await context.SaveChangesAsync();
            }

            var adminUser = new ApplicationUser
            {
                UserName = "admin@ecommerce.com",
                Email = "admin@ecommerce.com",
                EmailConfirmed = true,
                FirstName = "majed",
                LastName = "hndieh"
            };

            if (userManager.Users.All(u => u.Email != adminUser.Email))
            {
                var user = await userManager.FindByEmailAsync(adminUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(adminUser, "Admin@123");
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
    
}
