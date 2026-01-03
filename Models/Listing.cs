using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Project_1.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_1.Models
{
    public class Listing
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public string ImagePath { get; set; }
        public bool IsSold { get; set; } = false;

        [Required]
        public string? IdentityUserId { get; set; }
        [ForeignKey("IdentityUserId")]
        public IdentityUser? User { get; set; }

        public List<Bid>? Bids { get; set; }
        public List<Comment>? Comments { get; set; }

        public DateTime ClosingTime { get; set; }

        public decimal CurrentBid { get; set; }

        // --- THIS IS THE CORRECTED LINE ---
        // Added as nullable 'string?' to prevent migration errors on existing data.
        public string? Category { get; set; }

        // This 'Status' property is not mapped to the database
        // and is fine as-is.
        [NotMapped] // Good practice to add this
        public string Status
        {
            get
            {
                if (IsSold) return "Sold";
                if (DateTime.UtcNow > ClosingTime) return "Closed";
                return "Active"; // default
            }
        }

        public int? WinningBidId { get; set; }
    }
}