using WebApplication5.Data;
using WebApplication5.Models;

namespace WebApplication5.Repository
{
    public class CompetitorProductRepository : ICompetitorProductRepository
    {
        private readonly AppDbContext _context;

        public CompetitorProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CompetitorProduct> CreateAsync(CompetitorProduct product)
        {
            _context.CompetitorProducts.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }
    }
}
