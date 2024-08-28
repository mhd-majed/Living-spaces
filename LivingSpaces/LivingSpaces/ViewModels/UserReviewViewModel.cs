using LivingSpaces.Models.LivingSpaces.Models;
using LivingSpaces.Models;

namespace LivingSpaces.ViewModels
{
    public class UserReviewViewModel
    {
        public ApplicationUser ApplicationUser { get; set; }
        public List<UserReview> UserReviews { get; set; }
    }
}
