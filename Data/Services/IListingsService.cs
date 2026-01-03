using Project_1.Models;

namespace Project_1.Data.Services
{
    public interface IListingsService
    {

        IQueryable<Listing> GetAll();
        Task Add(Listing listing);
        Task<Listing> GetById(int? id);
        Task SaveChanges();
        Task GetById(object listingId);
    }
}
