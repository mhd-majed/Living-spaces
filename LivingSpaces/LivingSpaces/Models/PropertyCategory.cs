using System.ComponentModel.DataAnnotations;


namespace LivingSpaces.Models
    
{


    public class PropertyCategory
    {
        [Key]
        public int CategoryID { get; set; }

        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(50, ErrorMessage = "Category name can't be longer than 50 characters.")]
        public string CategoryName { get; set; }

        [Required(ErrorMessage = "Category description is required.")]
        [DataType(DataType.MultilineText)]
        [StringLength(1000, ErrorMessage = "Description can't be longer than 1000 characters.")]
        public string CategoryDescription { get; set; }

        public bool IsDeleted { get; set; }

        public virtual ICollection<PropertyListing>? PropertyListings { get; set; }
    }
}
