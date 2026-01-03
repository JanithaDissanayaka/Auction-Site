using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_1.Models
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        public int ListingId { get; set; }  // FK to Listing

        [ForeignKey("ListingId")]
        public Listing Listing { get; set; }  // ← Add this

        [Required]
        public string UserId { get; set; }   // ID of the user who paid

        [Required]
        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    }
}
