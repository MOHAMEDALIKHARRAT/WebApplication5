using Microsoft.EntityFrameworkCore;
using WebApplication5.Data;
using WebApplication5.Models;

namespace WebApplication5.Repository
{
    public class VisitRepository : IVisitRepository
    {
        private readonly AppDbContext _context;

        public VisitRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Visit> CreateAsync(Visit visit)
        {
            _context.Visits.Add(visit);
            await _context.SaveChangesAsync();
            return visit;
        }

        public async Task<Visit?> GetByIdAsync(int id)
        {
            return await _context.Visits
                .Include(v => v.Commercial)
                .Include(v => v.Client)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<List<Visit>> GetByCommercialAsync(string commercialCref)
        {
            return await _context.Visits
                .Include(v => v.Client)
                .Where(v => v.CommercialCref == commercialCref)
                .ToListAsync();
        }

        public async Task UpdateAsync(Visit visit)
        {
            _context.Visits.Update(visit);
            await _context.SaveChangesAsync();
        }
    }
}
