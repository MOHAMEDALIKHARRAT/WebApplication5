using WebApplication5.Models;

namespace WebApplication5.Repository
{
    public interface IVisitRepository
    {
        Task<Visit> CreateAsync(Visit visit);
        Task<Visit?> GetByIdAsync(int id);
        Task<List<Visit>> GetByCommercialAsync(string commercialCref);
        Task UpdateAsync(Visit visit);
    }
}
