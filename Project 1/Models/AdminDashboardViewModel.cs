using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Project_1.Models
{
    public class AdminDashboardViewModel
    {
        // All listings in the system, including bids
        public List<Listing> Listings { get; set; } = new List<Listing>();

        // All users in the system
        public List<IdentityUser> Users { get; set; } = new List<IdentityUser>();
    }
}
