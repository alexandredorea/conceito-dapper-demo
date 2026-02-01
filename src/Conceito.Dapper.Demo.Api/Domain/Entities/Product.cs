namespace Conceito.Dapper.Demo.Api.Domain.Entities;

/// <summary>
/// Representa um produto no sistema
/// </summary>
public sealed class Product
{
    public int Id { get; set; }

    public string Name { get; init; } = string.Empty; // Ou required string Nome

    public decimal Price { get; init; }

    public int Stock { get; init; }

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;  // Usar UTC em produção!

    public Category Category { get; set; } = null!;

    // Propriedade calculada - não existe no banco
    public bool HasStock => Stock > 0;
}