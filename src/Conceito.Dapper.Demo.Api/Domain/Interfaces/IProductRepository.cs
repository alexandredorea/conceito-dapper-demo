using Conceito.Dapper.Demo.Api.Domain.Entities;

namespace Conceito.Dapper.Demo.Api.Domain.Interfaces;

/// <summary>
/// Contrato para operações de banco relacionadas a Produto
/// </summary>
public interface IProductRepository
{
    // LEITURA: Queries que buscam dados
    Task<IEnumerable<Product>> GetAllAsync();

    Task<Product?> GetByIdAsync(int id);

    Task<IEnumerable<Product>> GetWithLowStockAsync(int minimumStock);

    Task<int> CountTotalAsync();

    Task<decimal> GetTotalStockValueAsync();

    Task<IEnumerable<Product>> GetWithCategoriesAsync();

    // ESCRITA: Comandos que modificam dados
    Task<int> AddAsync(Product product);

    Task<bool> UpdateAsync(Product product);

    Task<bool> UpdatePriceAsync(int id, decimal novoPreco);

    Task<bool> DeleteAsync(int id);

    // VERIFICAÇÃO: Operações de validação
    Task<bool> ExistsAsync(int id);
}