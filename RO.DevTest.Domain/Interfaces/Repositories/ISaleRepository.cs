public interface ISaleRepository
{
    Task<IEnumerable<Sale>> GetAllAsync(int page, int size);
    Task<IEnumerable<Sale>> GetByUserAsync(string userId);
    Task<Sale?> GetByIdAsync(Guid id);
    Task AddAsync(Sale sale);
}
