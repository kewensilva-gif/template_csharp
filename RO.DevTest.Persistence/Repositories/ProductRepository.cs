using Microsoft.EntityFrameworkCore;

namespace RO.DevTest.Persistence.Repositories;

public class ProductRepository : BaseRepository<Product>, IProductRepository
{
    public ProductRepository(DefaultContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Product>> GetAllAsync(int page = 0, int size = 2) =>
        await Context.Set<Product>()
        .Skip(size * page)
        .Take(size)
        .ToListAsync();
    public async Task<Product?> GetByIdAsync(Guid id) =>
        await Context.Set<Product>().FindAsync(id);

    public async Task AddAsync(Product product) =>
        await CreateAsync(product);

    public async Task UpdateAsync(Product product)
    {
        Context.Set<Product>().Update(product);
        await Context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var product = await GetByIdAsync(id);
        if (product is null) return;
        Context.Set<Product>().Remove(product);
        await Context.SaveChangesAsync();
    }
}
