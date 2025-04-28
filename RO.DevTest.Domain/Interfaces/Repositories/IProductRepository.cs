using RO.DevTest.Domain.Entities;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync(int page, int size);
    Task<Product?> GetByIdAsync(Guid id);
    Task AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(Guid id);
}
