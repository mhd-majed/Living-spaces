namespace LivingSpaces.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    namespace LivingSpaces.Models
    {
        public class UserReview
        {
            public int UserReviewID { get; set; }

            [Required]
            [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
            public int Rating { get; set; }

            [StringLength(1000, ErrorMessage = "Review cannot be longer than 1000 characters.")]
            public string ReviewText { get; set; }

            [Required]
            public string ReviewerID { get; set; }

            [ForeignKey("ReviewerID")]
            public virtual ApplicationUser Reviewer { get; set; }

            [Required]
            public string ReviewedUserID { get; set; }

            [ForeignKey("ReviewedUserID")]
            public virtual ApplicationUser ReviewedUser { get; set; }

            public DateTime CreatedAt { get; set; } = DateTime.Now;
            public bool IsDeleted { get; set; } = false;

        }
    }

}
