using Microsoft.EntityFrameworkCore;
using RO.DevTest.Domain.Entities;
using RO.DevTest.Domain.Interfaces.Repositories;

namespace RO.DevTest.Persistence.Repositories;

public class ProductRepository : BaseRepository<Product>, IProductRepository
{
    public ProductRepository(DefaultContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Product>> GetAllAsync() =>
        await Context.Set<Product>().ToListAsync();

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
