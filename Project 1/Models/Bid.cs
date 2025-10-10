using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Project_1.Models
{
    public class Bid
    {
        public int Id { get; set; }

        public double Price { get; set; }

        [Required]
        public string? IdentityUserId { get; set; }

        [ForeignKey("IdentityUserId")]
        public IdentityUser? User { get; set; }

        public int? ListingId { get; set; }

        [ForeignKey("ListingId")]
        public Listing? Listing { get; set; }

        // ✅ New property for admin approval
        public bool IsApproved { get; set; } = false;

        // Optional: timestamp for bid
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
