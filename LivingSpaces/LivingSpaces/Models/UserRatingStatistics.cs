using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LivingSpaces.Models
{
    public class UserRatingStatistics
    {
        [Key]
        public string UserID { get; set; }

        [ForeignKey("UserID")]
        public virtual ApplicationUser User { get; set; }

        public double AverageRating { get; set; } = 0;
        public int RatingCount { get; set; } = 0;

    }

}
