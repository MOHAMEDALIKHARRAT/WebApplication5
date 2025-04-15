using WebApplication5.Models;

namespace WebApplication5.Repository
{
    public interface IQuoteRepository
    {
        Task<Quote> CreateAsync(Quote quote);
        Task<Quote?> GetByIdAsync(int id);
    }

}

