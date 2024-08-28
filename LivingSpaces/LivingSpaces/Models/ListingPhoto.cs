using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LivingSpaces.Models
{
    public class ListingPhoto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string PhotoUrl { get; set; }

        // Foreign key to PropertyListing
        [Required]
        public int PropertyListingID { get; set; }

        [ForeignKey("PropertyListingID")]
        public virtual PropertyListing PropertyListing { get; set; }

    }
}
