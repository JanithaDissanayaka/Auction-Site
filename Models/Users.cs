using System.ComponentModel.DataAnnotations;

namespace Project_1.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public string Role { get; set; } // e.g., Buyer, Seller, Admin

        public bool IsActive { get; set; } = true;
    }
}
