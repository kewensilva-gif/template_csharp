using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RO.DevTest.Domain.Entities;
using RO.DevTest.Domain.Interfaces.Repositories;

namespace RO.DevTest.WebApi.Controllers;

// [Authorize]
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
    public async Task<IActionResult> Get()
    {
        var customers = await _customerRepository.GetAllAsync();
        return Ok(customers);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer is null)
            return NotFound();

        return Ok(customer);
    }

    // [Authorize(Roles = "Admin")] // apenas admin pode criar
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
