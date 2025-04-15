using WebApplication5.Models;

namespace WebApplication5.Repository
{
    public interface ICompetitorProductRepository
    {
        Task<CompetitorProduct> CreateAsync(CompetitorProduct product);

    }
}
