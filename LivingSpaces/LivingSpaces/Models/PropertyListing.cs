using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

namespace LivingSpaces.Models
{
    public enum PropertyType
    {
        Selling,
        Renting
    }

    public class PropertyListing
    {
        public int PropertyListingID { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(1000, ErrorMessage = "Description cannot be longer than 1000 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive number.")]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Bedrooms must be a non-negative number.")]
        public int Bedrooms { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Bathrooms must be a non-negative number.")]
        public int Bathrooms { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Square meter must be a positive number.")]
        public double SquareMeter { get; set; }

        public string? VRUrl { get; set; }

        [DataType(DataType.Date)]
        public DateTime ListedAt { get; set; } = DateTime.Now;

        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Property type is required.")]
        public PropertyType PropertyType { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsAvailable { get; set; } = true;

        public bool IsDeleted { get; set; } = false;

        [Required]
        public int CategoryID { get; set; }

        [ForeignKey("CategoryID")]
        public virtual PropertyCategory PropertyCategory { get; set; }

        [Required]
        public string UserID { get; set; }

        [ForeignKey("UserID")]
        public virtual ApplicationUser ApplicationUser { get; set; }

        public virtual ICollection<ListingPhoto> ListingPhotos { get; set; } = new List<ListingPhoto>();

        // Add Address navigation property
        public int? AddressID { get; set; }

        [ForeignKey("AddressID")]
        public virtual Address Address { get; set; }

        public virtual Booking Booking { get; set; }

    }
}
