using WebApplication5.Models;

namespace WebApplication5.Repository
{
    public interface ISaleRepository
    {
        Task<IEnumerable<Sale>> GetAllAsync();
        Task<Sale?> GetByIdAsync(int id);
        Task<Sale> AddAsync(Sale sale);
        Task<bool> UpdateAsync(Sale sale);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Tiers>> GetTiersByCommercialAsync(string commercialId);
    }
}