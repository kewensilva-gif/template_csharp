using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RO.DevTest.Domain.Entities;
using RO.DevTest.Domain.Interfaces.Repositories;
using System.Security.Claims;

namespace RO.DevTest.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SaleController : ControllerBase
{
    private readonly ISaleRepository _saleRepository;

    public SaleController(ISaleRepository saleRepository)
    {
        _saleRepository = saleRepository;
    }

    // Somente Admin vê todas as vendas
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var sales = await _saleRepository.GetAllAsync();
        return Ok(sales);
    }

    // Usuário autenticado vê as próprias vendas
    [HttpGet("me")]
    public async Task<IActionResult> GetMySales()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var sales = await _saleRepository.GetByUserAsync(userId!);
        return Ok(sales);
    }

    // Ver uma venda específica
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var sale = await _saleRepository.GetByIdAsync(id);
        if (sale is null) return NotFound();
        return Ok(sale);
    }

    // Criar uma nova venda
    [HttpPost]
    public async Task<IActionResult> Create(Sale sale)
    {
        // Atribui o ID do usuário logado como vendedor
        sale.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        await _saleRepository.AddAsync(sale);
        return CreatedAtAction(nameof(GetById), new { id = sale.Id }, sale);
    }
}
