using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_1.Data;
using Project_1.Models;

namespace Project_1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Get top 3 featured listings (you can filter by IsSold == false if you want only open ones)
            var featuredListings = await _context.Listings
                .Include(l => l.Bids)
                .Where(l => !l.IsSold)
                .OrderByDescending(l => l.Id)
                .Take(3)
                .ToListAsync();

            return View(featuredListings);
        }
        public IActionResult AboutUs()
        {
            return View();
        }

    }
}
