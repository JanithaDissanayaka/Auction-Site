using Project_1.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project_1.Data.Services
{
    public interface IBidsService
    {
        Task Add(Bid bid);                  // Add a new bid
        IQueryable<Bid> GetAll();           // Get all bids
        Task<List<Bid>> GetWonBidsAsync(string userId); // Get all bids the user has won
    }
}
