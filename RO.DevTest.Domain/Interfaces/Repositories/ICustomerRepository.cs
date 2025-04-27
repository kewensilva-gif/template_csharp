using RO.DevTest.Domain.Entities;
using System.Linq.Expressions;

namespace RO.DevTest.Domain.Interfaces.Repositories
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<Customer>> GetAllAsync(int page, int size);
        Task<Customer?> GetByIdAsync(Guid id);
        Task<IEnumerable<Customer>> GetAsync(Expression<Func<Customer, bool>> predicate);
        Task AddAsync(Customer customer);
        Task UpdateAsync(Customer customer);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Expression<Func<Customer, bool>> predicate);
    }
}
