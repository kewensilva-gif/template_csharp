using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RO.DevTest.Persistence.Repositories;
using System.Security.Claims;

namespace RO.DevTest.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SaleController : ControllerBase
{
    private readonly ISaleRepository _saleRepository;

    public SaleController(ISaleRepository saleRepository)
    {
        _saleRepository = saleRepository;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAll(int page = 0, int size = 10)
    {
        var sales = await _saleRepository.GetAllAsync(page, size);
        var totalRecords = sales.Count();
        var pagedSales = new PagedResult<Sale>(sales, page, size, totalRecords);
        return Ok(pagedSales);
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMySales()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var sales = await _saleRepository.GetByUserAsync(userId!);
        return Ok(sales);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var sale = await _saleRepository.GetByIdAsync(id);
        if (sale is null) return NotFound();
        return Ok(sale);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Sale sale)
    {
        sale.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        await _saleRepository.AddAsync(sale);
        return CreatedAtAction(nameof(GetById), new { id = sale.Id }, sale);
    }
}
