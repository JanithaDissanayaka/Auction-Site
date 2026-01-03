using Project_1.Models;

namespace Project_1.Data.Services
{
    public interface ICommentsService
    {
        Task Add(Comment comment);
    }
}