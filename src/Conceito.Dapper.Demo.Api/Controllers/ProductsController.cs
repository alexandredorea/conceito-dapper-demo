using Conceito.Dapper.Demo.Api.Domain.Entities;
using Conceito.Dapper.Demo.Api.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Conceito.Dapper.Demo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ProductsController(
    ILogger<ProductsController> logger,
    IProductRepository repository) : ControllerBase
{
    /// <summary>
    /// Lista todos os produtos
    /// GET: api/products
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        var products = await repository.GetAllAsync();
        return Ok(products);
    }

    /// <summary>
    /// Busca um produto específico
    /// GET: api/products/5
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetByIdAsync(
        [FromRoute] int id)
    {
        var product = await repository.GetByIdAsync(id);

        if (product is null)
            return NotFound(new { message = $"Produto {id} não encontrado" });

        return Ok(product);
    }

    /// <summary>
    /// Cria um novo produto
    /// POST: api/produtos
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> AddAsync(
        [FromBody] Product product)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var id = await repository.AddAsync(product);

        return CreatedAtAction(
            nameof(GetByIdAsync),
            new { id },
            new { id, message = "Produto criado com sucesso" }
        );
    }

    /// <summary>
    /// Atualiza um produto existente
    /// PUT: api/products/5
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateAsync(
        [FromRoute] int id,
        [FromBody] Product product)
    {
        if (id != product.Id)
            return BadRequest(new { message = "ID não corresponde" });

        var sucesso = await repository.UpdateAsync(product);

        if (!sucesso)
            return NotFound(new { message = $"Produto {id} não encontrado" });

        return NoContent();
    }

    /// <summary>
    /// Deleta um produto
    /// DELETE: api/products/5
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAsync(
        [FromRoute] int id)
    {
        var success = await repository.DeleteAsync(id);

        if (!success)
            return NotFound(new { message = $"Produto {id} não encontrado" });

        return NoContent();
    }

    /// <summary>
    /// Lista produtos com estoque baixo
    /// GET: api/products/low-stock?minimum=10
    /// </summary>
    [HttpGet("low-stock")]
    public async Task<IActionResult> GetWithLowStockAsync(
        [FromQuery] int minimum = 5)
    {
        var products = await repository.GetWithLowStockAsync(minimum);
        return Ok(products);
    }
}