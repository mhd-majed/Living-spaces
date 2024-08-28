using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LivingSpaces.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using LivingSpaces.Models.LivingSpaces.Models;

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public ApplicationDbContext (DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<LivingSpaces.Models.PropertyCategory> Category { get; set; } = default!;
        public DbSet<LivingSpaces.Models.PropertyListing> Properties { get; set; } = default!;
        public DbSet<LivingSpaces.Models.SubscriptionPlan> SubscriptionPlans { get; set; }
        public DbSet<LivingSpaces.Models.UserSubscription> UserSubscriptions { get; set; }
        public DbSet<LivingSpaces.Models.ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<LivingSpaces.Models.ListingPhoto> ListingPhoto { get; set; } = default!;
        public DbSet<Text> Texts { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<UserReview> UserReviews { get; set; }
        public DbSet<UserRatingStatistics> UserRatingStatistics { get; set; }

        public DbSet<Testimonial> Testimonials { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<DailyBooking> DailyBookings { get; set; }
        public DbSet<WeeklyBooking> WeeklyBookings { get; set; }
    public DbSet<Tour> Tours { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserReview>()
            .HasOne(ur => ur.Reviewer)
            .WithMany(u => u.ReviewsWritten)
            .HasForeignKey(ur => ur.ReviewerID)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UserReview>()
            .HasOne(ur => ur.ReviewedUser)
            .WithMany(u => u.ReviewsReceived)
            .HasForeignKey(ur => ur.ReviewedUserID)
            .OnDelete(DeleteBehavior.Restrict);



        modelBuilder.Entity<Booking>()
           .HasOne(b => b.DailyBooking)
           .WithOne()
           .HasForeignKey<Booking>(b => b.DailyBookingID)
           .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Booking>()
            .HasOne(b => b.WeeklyBooking)
            .WithOne()
            .HasForeignKey<Booking>(b => b.WeeklyBookingID)
            .OnDelete(DeleteBehavior.Cascade);
    }

}
