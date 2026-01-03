using Microsoft.EntityFrameworkCore;
using Project_1.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project_1.Data.Services
{
    public class BidsService : IBidsService
    {
        private readonly ApplicationDbContext _context;

        public BidsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Add(Bid bid)
        {
            _context.Bids.Add(bid);
            await _context.SaveChangesAsync();
        }

        public IQueryable<Bid> GetAll()
        {
            return _context.Bids
                           .Include(b => b.Listing)
                           .ThenInclude(l => l.User);
        }

        public async Task<List<Bid>> GetWonBidsAsync(string userId)
        {
            // Only include bids for listings that are sold
            return await _context.Bids
                .Include(b => b.Listing)
                .Where(b => b.IdentityUserId == userId && b.Listing.IsSold)
                .OrderByDescending(b => b.Listing.ClosingTime)
                .ToListAsync();
        }
    }
}
