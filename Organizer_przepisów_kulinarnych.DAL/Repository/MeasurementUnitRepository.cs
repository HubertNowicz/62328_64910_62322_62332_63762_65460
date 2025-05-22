using Microsoft.EntityFrameworkCore;
using Organizer_przepisów_kulinarnych.DAL.DbContexts;
using Organizer_przepisów_kulinarnych.DAL.Interfaces;


namespace Organizer_przepisów_kulinarnych.DAL.Repository
{
    public class MeasurementUnitRepository : IMeasurementUnitRepository
    {
        private readonly ApplicationDbContext _context;

        public MeasurementUnitRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<MeasurementUnit>> GetAllAsync()
        {
            return await _context.MeasurementUnits.ToListAsync();
        }
    }
}
