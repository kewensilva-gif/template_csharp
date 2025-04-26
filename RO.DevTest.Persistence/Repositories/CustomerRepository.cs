using Microsoft.EntityFrameworkCore;
using RO.DevTest.Domain.Entities;
using RO.DevTest.Domain.Interfaces.Repositories;
using System.Linq.Expressions;

namespace RO.DevTest.Persistence.Repositories;

public class CustomerRepository : ICustomerRepository
{
    protected readonly DefaultContext Context;

    public CustomerRepository(DefaultContext context)
    {
        Context = context;
    }

    public async Task<IEnumerable<Customer>> GetAllAsync() =>
        await Context.Set<Customer>().ToListAsync();

    public async Task<Customer?> GetByIdAsync(Guid id) =>
        await Context.Set<Customer>().FindAsync(id);

    public async Task<IEnumerable<Customer>> GetAsync(Expression<Func<Customer, bool>> predicate) =>
        await Context.Set<Customer>().Where(predicate).ToListAsync();

    public async Task AddAsync(Customer customer)
    {
        await Context.Set<Customer>().AddAsync(customer);
        await Context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Customer customer)
    {
        Context.Set<Customer>().Update(customer);
        await Context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var customer = await GetByIdAsync(id);
        if (customer is null) return;
        Context.Set<Customer>().Remove(customer);
        await Context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(Expression<Func<Customer, bool>> predicate) =>
        await Context.Set<Customer>().AnyAsync(predicate);
}