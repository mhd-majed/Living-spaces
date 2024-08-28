using System.ComponentModel.DataAnnotations;

namespace LivingSpaces.Models
{
    public class SubmitReviewViewModel
    {
        public string ReviewedUserID { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }

        [StringLength(1000, ErrorMessage = "Review cannot be longer than 1000 characters.")]
        public string ReviewText { get; set; }
    }
}
