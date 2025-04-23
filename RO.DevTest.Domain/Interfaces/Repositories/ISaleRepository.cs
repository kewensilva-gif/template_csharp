using RO.DevTest.Domain.Entities;

public interface ISaleRepository
{
    Task<IEnumerable<Sale>> GetAllAsync();
    Task<Sale?> GetByIdAsync(Guid id);
    Task AddAsync(Sale sale);
}
