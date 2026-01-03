using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_1.Models
{
    public class ListingVM
    {
        // Properties that ARE on your form:
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public double Price { get; set; }

        [Required]
        [Display(Name = "Item Image")]
        public IFormFile Image { get; set; }

        // --- THIS IS THE MISSING PROPERTY ---
        [Required(ErrorMessage = "Please select a category")]
        public string Category { get; set; }
        // --- END ---

        // Auction duration fields
        public int ClosingHours { get; set; }
        public int ClosingMinutes { get; set; }
        public int ClosingDays { get; set; }

        // This is set by the hidden field in your form
        public string? IdentityUserId { get; set; }


        // --- REMOVE THESE PROPERTIES ---
        // 'id' is created by the database, not the form.
        // 'IsSold' is set by the controller, not the form.
        // 'User' is a database navigation property, not part of a VM.
    }
}