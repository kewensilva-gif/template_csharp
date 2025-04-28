using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RO.DevTest.Domain.Interfaces.Repositories;
using RO.DevTest.Persistence.Repositories;

namespace RO.DevTest.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerController : ControllerBase
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerController(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Get(int page = 0, int size = 10)
    {
        var customers = await _customerRepository.GetAllAsync(page, size);
        var totalRecords = customers.Count();
        var pagedCustomers = new PagedResult<Customer>(customers, page, size, totalRecords);
        return Ok(pagedCustomers);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer is null)
            return NotFound();
        
        return Ok(customer);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(Customer customer)
    {
        await _customerRepository.AddAsync(customer);
        return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, Customer customer)
    {
        if (id != customer.Id) return BadRequest();

        await _customerRepository.UpdateAsync(customer);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _customerRepository.DeleteAsync(id);
        return NoContent();
    }
}
