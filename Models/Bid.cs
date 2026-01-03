using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        // ✅ New property to track payment
        public bool IsPaid { get; set; } = false;

        // Optional: timestamp for bid
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
