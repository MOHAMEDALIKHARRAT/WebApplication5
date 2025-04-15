using WebApplication5.Models;

namespace WebApplication5.Repository
{
    public interface IRecoveryRepository
    {

        Task<Recovery> CreateAsync(Recovery recovery);
    }
}
