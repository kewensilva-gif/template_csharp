using Microsoft.EntityFrameworkCore;
using RO.DevTest.Domain.Entities;
using RO.DevTest.Domain.Interfaces.Repositories;

namespace RO.DevTest.Persistence.Repositories;

public class SaleRepository : BaseRepository<Sale>, ISaleRepository
{
    private readonly DefaultContext _context;

    public SaleRepository(DefaultContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Sale>> GetAllAsync()
    {
        return await _context.Sales
            .Include(s => s.Customer)
            .Include(s => s.Items)
                .ThenInclude(i => i.Product)
            .Include(s => s.User)
            .ToListAsync();
    }

    public async Task<IEnumerable<Sale>> GetByUserAsync(string userId)
    {
        return await _context.Sales
            .Where(s => s.UserId == userId)
            .Include(s => s.Customer)
            .Include(s => s.Items)
                .ThenInclude(i => i.Product)
            .ToListAsync();
    }

    public async Task<Sale?> GetByIdAsync(Guid id)
    {
        return await _context.Sales
            .Include(s => s.Customer)
            .Include(s => s.Items)
                .ThenInclude(i => i.Product)
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task AddAsync(Sale sale)
    {
        await _context.Sales.AddAsync(sale);
        await _context.SaveChangesAsync();
    }
}
