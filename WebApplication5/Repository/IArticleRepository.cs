using WebApplication5.Models;

namespace WebApplication5.Repository
{
    public interface IArticleRepository
    {
        Task<Article?> GetByIdAsync(int id);
        Task<List<Article>> GetAllAsync();
    }
}