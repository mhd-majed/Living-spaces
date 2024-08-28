using LivingSpaces.Models;
using LivingSpaces.Models.LivingSpaces.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace LivingSpaces.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "First name is required.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        public string LastName { get; set; }

        public string ProfileImageUrl { get; set; } = "~/assets/images/user.jpg";

        public DateTime CreatedAt { get; set; }= DateTime.Now;
        public bool IsDeleted { get; set; } = false;

        public virtual UserSubscription? UserSubscription { get; set; } 

        public virtual UserRatingStatistics Statistics { get; set; } = new UserRatingStatistics();


        public virtual ICollection<UserReview> ReviewsWritten { get; set; } = new List<UserReview>();

        public virtual ICollection<UserReview> ReviewsReceived { get; set; } = new List<UserReview>();
    }
}