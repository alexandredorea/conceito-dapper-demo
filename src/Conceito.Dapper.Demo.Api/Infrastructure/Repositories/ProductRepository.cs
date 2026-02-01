using Conceito.Dapper.Demo.Api.Domain.Entities;
using Conceito.Dapper.Demo.Api.Domain.Interfaces;
using Conceito.Dapper.Demo.Api.Infrastructure.Exceptions;
using Conceito.Dapper.Demo.Api.Infrastructure.Queries;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Conceito.Dapper.Demo.Api.Infrastructure.Repositories;

internal sealed class ProductRepository(
    IDbConnectionFactory connectionFactory,
    ILogger<ProductRepository> logger) : IProductRepository
{
    /// <summary>
    /// Busca todos os produtos do banco
    /// USO: Listagens simples, relatórios
    /// </summary>
    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        try
        {
            // 1. Criar conexão com o banco
            using var db = connectionFactory.CreateConnection();

            // 2. Executar query e mapear para lista de Produto
            // O Dapper faz o mapeamento automático por nome das colunas
            var products = await db.QueryAsync<Product>(ProductQueries.GetAll);

            logger.LogDebug("Buscados {Count} produtos do banco", products.Count());

            return products;
        }
        catch (SqlException ex)
        {
            logger.LogError(ex, "Erro ao buscar todos os produtos");
            throw new RepositoryException("Erro ao obter produtos", ex);
        }
    }

    /// <summary>
    /// Busca um produto específico por ID
    /// USO: Detalhes de produto, validações
    /// RETORNO: null se não encontrar
    /// </summary>
    public async Task<Product?> GetByIdAsync(int id)
    {
        try
        {
            using var db = connectionFactory.CreateConnection();

            // QueryFirstOrDefaultAsync retorna:
            // - O primeiro registro encontrado
            // - null se não encontrar nenhum
            //
            // new { Id = id } cria um objeto anônimo com os parâmetros
            // O Dapper substitui @Id na query pelo valor
            var product = await db.QueryFirstOrDefaultAsync<Product>(
                ProductQueries.GetById,
                new { Id = id }
            );

            if (product is null)
                logger.LogWarning("Produto com ID {ProdutoId} não encontrado", id);

            return product;
        }
        catch (SqlException ex)
        {
            logger.LogError(ex, "Erro ao buscar produto com ID {ProdutoId}", id);
            throw new RepositoryException($"Erro ao buscar produto {id}", ex);
        }
    }

    /// <summary>
    /// Busca produtos com estoque abaixo do mínimo
    /// USO: Alertas de reposição, relatórios gerenciais
    /// </summary>
    public async Task<IEnumerable<Product>> GetWithLowStockAsync(int minimumStock)
    {
        try
        {
            using var db = connectionFactory.CreateConnection();

            var products = await db.QueryAsync<Product>(
                ProductQueries.GetWithLowStock,
                new { Stock = minimumStock }
            );

            logger.LogInformation(
                "Encontrados {Count} produtos com estoque <= {Minimo}",
                products.Count(),
                minimumStock
            );

            return products;
        }
        catch (SqlException ex)
        {
            logger.LogError(ex, "Erro ao buscar produtos com estoque baixo");
            throw new RepositoryException($"Erro ao buscar produtos com estoque baixo", ex);
        }
    }

    /// <summary>
    /// Conta o total de produtos cadastrados
    /// USO: Paginação, dashboards, estatísticas
    /// </summary>
    public async Task<int> CountTotalAsync()
    {
        try
        {
            using var db = connectionFactory.CreateConnection();

            // ExecuteScalarAsync retorna um único valor (a contagem)
            var total = await db.ExecuteScalarAsync<int>(ProductQueries.CountTotal);

            return total;
        }
        catch (SqlException ex)
        {
            logger.LogError(ex, "Erro ao contar total de produtos");
            throw new RepositoryException($"Erro ao contar total de produtos", ex);
        }
    }

    /// <summary>
    /// Calcula o valor total do estoque
    /// USO: Relatórios financeiros, balanços
    /// </summary>
    public async Task<decimal> GetTotalStockValueAsync()
    {
        try
        {
            using var db = connectionFactory.CreateConnection();

            // Pode retornar null se não houver produtos
            var valor = await db.ExecuteScalarAsync<decimal?>(
                ProductQueries.TotalStockValue
            );

            return valor ?? 0;
        }
        catch (SqlException ex)
        {
            logger.LogError(ex, "Erro ao calcular valor total do estoque");
            throw new RepositoryException($"Erro ao calcular valor total do estoque", ex);
        }
    }

    /// <summary>
    /// Adiciona um novo produto ao banco
    /// RETORNO: ID do produto criado
    /// </summary>
    public async Task<int> AddAsync(Product product)
    {
        try
        {
            using var db = connectionFactory.CreateConnection();

            // ExecuteScalarAsync porque a query retorna o ID gerado
            var id = await db.ExecuteScalarAsync<int>(
                ProductQueries.Insert,
                new
                {
                    product.Name,
                    product.Price,
                    product.Stock,
                    product.CreatedAt
                }
            );

            logger.LogInformation(
                "Produto '{NomeProduto}' adicionado com ID {ProdutoId}",
                product.Name,
                id
            );

            return id;
        }
        catch (SqlException ex)
        {
            logger.LogError(
                ex,
                "Erro ao adicionar produto '{NomeProduto}'",
                product.Name
            );
            throw new RepositoryException("Erro ao adicionar produto", ex);
        }
    }

    /// <summary>
    /// Atualiza um produto existente
    /// RETORNO: true se atualizou, false se o produto não existe
    /// </summary>
    public async Task<bool> UpdateAsync(Product product)
    {
        try
        {
            using var db = connectionFactory.CreateConnection();

            // ExecuteAsync retorna o número de linhas afetadas
            var rowsAffected = await db.ExecuteAsync(
                ProductQueries.Update,
                new
                {
                    product.Id,
                    product.Name,
                    product.Price,
                    product.Stock
                }
            );

            var success = rowsAffected > 0;
            if (success)
            {
                logger.LogInformation(
                    "Produto {ProdutoId} atualizado com sucesso",
                    product.Id
                );
            }
            else
            {
                logger.LogWarning(
                    "Tentativa de atualizar produto inexistente {ProdutoId}",
                    product.Id
                );
            }

            return success;
        }
        catch (SqlException ex)
        {
            logger.LogError(
                ex,
                "Erro ao atualizar produto {ProdutoId}",
                product.Id
            );
            throw new RepositoryException($"Erro ao atualizar produto {product.Id}", ex);
        }
    }

    /// <summary>
    /// Atualiza apenas o preço de um produto
    /// USO: Ajustes de preço frequentes sem recarregar todo objeto
    /// RETORNO: true se atualizou, false se não encontrou o produto
    /// </summary>
    public async Task<bool> UpdatePriceAsync(int id, decimal newPrice)
    {
        try
        {
            using var db = connectionFactory.CreateConnection();

            var rowsAffected = await db.ExecuteAsync(
                ProductQueries.UpdatePrice,
                new { Id = id, Price = newPrice }
            );

            var success = rowsAffected > 0;

            if (success)
            {
                logger.LogInformation(
                    "Preço do produto {ProdutoId} atualizado para {NovoPreco:C}",
                    id,
                    newPrice
                );
            }

            return success;
        }
        catch (SqlException ex)
        {
            logger.LogError(
                ex,
                "Erro ao atualizar preço do produto {ProdutoId}",
                id
            );
            throw new RepositoryException($"Erro ao atualizar produto {id}", ex);
        }
    }

    /// <summary>
    /// Remove um produto do banco
    /// RETORNO: true se deletou, false se não encontrou
    /// </summary>
    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            using var db = connectionFactory.CreateConnection();

            var rowsAffected = await db.ExecuteAsync(
                ProductQueries.Delete,
                new { Id = id }
            );

            var success = rowsAffected > 0;

            if (success)
            {
                logger.LogInformation("Produto {ProdutoId} deletado", id);
            }
            else
            {
                logger.LogWarning(
                    "Tentativa de deletar produto inexistente {ProdutoId}",
                    id
                );
            }

            return success;
        }
        catch (SqlException ex)
        {
            logger.LogError(ex, "Erro ao deletar produto {ProdutoId}", id);
            throw new RepositoryException($"Erro ao deletar produto {id}", ex);
        }
    }

    /// <summary>
    /// Verifica se um produto existe no banco
    /// USO: Validações antes de operações
    /// </summary>
    public async Task<bool> ExistsAsync(int id)
    {
        if (id <= 0) return false;

        try
        {
            using var db = connectionFactory.CreateConnection();
            var count = await db.ExecuteScalarAsync<int>(ProductQueries.Exists, new { Id = id });

            return count > 0;
        }
        catch (SqlException ex)
        {
            logger.LogError(ex, "Erro ao verificar existência do produto {ProdutoId}", id);
            throw new RepositoryException($"Erro ao verificar existência do produto {id}", ex);
        }
    }

    public async Task<IEnumerable<Product>> GetWithCategoriesAsync()
    {
        using var db = connectionFactory.CreateConnection();

        // Multi-mapping: mapeia para Produto E Categoria
        var products = await db.QueryAsync<Product, Category, Product>(
            ProductQueries.GetWithCategories,
            (produto, categoria) =>
            {
                produto.Category = categoria;
                return produto;
            },
            splitOn: "Id" // Onde começa o mapeamento da Categoria
        );

        return products;
    }
}