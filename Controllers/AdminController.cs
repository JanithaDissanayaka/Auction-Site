using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project_1.Data;
using Project_1.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Project_1.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Dashboard page
        public IActionResult Dashboard()
        {
            return View();
        }

        // Manage Users
        public IActionResult Users()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        // POST: Deactivate user (prevent login)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            // Lock user indefinitely
            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
            return RedirectToAction(nameof(Users));
        }

        // POST: Activate user (allow login)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            // Remove lockout
            await _userManager.SetLockoutEndDateAsync(user, null);
            return RedirectToAction(nameof(Users));
        }

        // POST: Delete user permanently
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            await _userManager.DeleteAsync(user);
            return RedirectToAction(nameof(Users));
        }

        // Manage Listings
        public IActionResult Listings()
        {
            var listings = _context.Listings.ToList();
            return View(listings);
        }

        // Manage Bids
        public IActionResult Bids()
        {
            var bids = _context.Bids.ToList();
            return View(bids);
        }

        // Manage Payments
        public IActionResult Payments()
        {
            var payments = _context.Payments.ToList();
            return View(payments);
        }

        public IActionResult EditListing(int id)
        {
            // Redirect to ListingsController's Edit action
            return RedirectToAction("Edit", "Listings", new { id });
        }

    }
}
