using System;
using System.ComponentModel.DataAnnotations;

namespace LivingSpaces.Models
{
    public class Text
    {
        public int Id { get; set; }

        [Required]
        public string RecipientId { get; set; }
        public virtual ApplicationUser Recipient { get; set; }

        [Required]
        public string SenderId { get; set; }
        public virtual ApplicationUser Sender { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime SentAt { get; set; } = DateTime.UtcNow;

    }
}
