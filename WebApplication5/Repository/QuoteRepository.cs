using Microsoft.EntityFrameworkCore;
using WebApplication5.Data;
using WebApplication5.Models;

namespace WebApplication5.Repository
{
    public class QuoteRepository : IQuoteRepository
    {
        private readonly AppDbContext _context;

        public QuoteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Quote> CreateAsync(Quote quote)
        {
            _context.Quotes.Add(quote);
            await _context.SaveChangesAsync();
            return quote;
        }

        public async Task<Quote?> GetByIdAsync(int id)
        {
            return await _context.Quotes
                .Include(q => q.QuoteLines)
                .ThenInclude(ql => ql.Article)
                .Include(q => q.Visit)
                .FirstOrDefaultAsync(q => q.Id == id);
        }
    }
}
