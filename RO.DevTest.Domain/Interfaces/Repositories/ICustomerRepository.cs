using RO.DevTest.Domain.Entities;

public interface ICustomerRepository
{
    Task<IEnumerable<Customer>> GetAllAsync();
    Task<Customer?> GetByIdAsync(Guid id);
    Task AddAsync(Customer customer);
    Task UpdateAsync(Customer customer);
    Task DeleteAsync(Guid id);
}
