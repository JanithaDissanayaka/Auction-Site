using Microsoft.AspNetCore.Mvc;
using Project_1.Data;
using Project_1.Models;
using Microsoft.EntityFrameworkCore;

[Area("Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Dashboard()
    {
        var listings = await _context.Listings.Include(l => l.Bids).ToListAsync();
        var users = await _context.Users.ToListAsync();
        return View(new AdminDashboardViewModel { Listings = listings, Users = users });
    }

    // Approve Bid Example
    public async Task<IActionResult> ApproveBid(int id)
    {
        var bid = await _context.Bids.FindAsync(id);
        if (bid != null)
        {
            bid.IsApproved = true;
            await _context.SaveChangesAsync();
        }
        return RedirectToAction("Dashboard");
    }
}
