public interface ISaleRepository
{
    Task<IEnumerable<Sale>> GetAllAsync();
    Task<IEnumerable<Sale>> GetByUserAsync(string userId);
    Task<Sale?> GetByIdAsync(Guid id);
    Task AddAsync(Sale sale);
}
