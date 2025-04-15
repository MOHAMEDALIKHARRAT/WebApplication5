using WebApplication5.Data;
using WebApplication5.Models;

namespace WebApplication5.Repository
{
    public class RecoveryRepository : IRecoveryRepository
    {
        private readonly AppDbContext _context;

        public RecoveryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Recovery> CreateAsync(Recovery recovery)
        {
            _context.Recoveries.Add(recovery);
            await _context.SaveChangesAsync();
            return recovery;
        }
    }
}
