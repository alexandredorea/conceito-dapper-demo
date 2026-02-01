namespace Conceito.Dapper.Demo.Api.Infrastructure.Queries;

/// <summary>
/// Centraliza todas as queries SQL relacionadas a Produto
/// Facilita manutenção e evita SQL espalhado pelo código
/// </summary>
internal static class ProductQueries
{
    // LEITURA: Buscar todos os produtos
    internal const string GetAll = @"
            SELECT
                Id,
                Name,
                Price,
                Stock,
                CreatedAt
            FROM Products
            ORDER BY Name";

    // LEITURA: Buscar por ID específico
    internal const string GetById = @"
            SELECT
                Id,
                Name,
                Price,
                Stock,
                CreatedAt
            FROM Products
            WHERE Id = @Id";

    // LEITURA: Buscar produtos com estoque baixo
    internal const string GetWithLowStock = @"
            SELECT
                Id,
                Name,
                Price,
                Stock,
                CreatedAt
            FROM Products
            WHERE Stock <= @MinimumStock
            ORDER BY Stock ASC";

    // ESCRITA: Inserir novo produto
    internal const string Insert = @"
            INSERT INTO Products (Name, Price, Stock, CreatedAt)
            VALUES (@Name, @Price, @Stock, @CreatedAt);

            -- Retorna o ID do registro inserido
            SELECT CAST(SCOPE_IDENTITY() as int);";

    // ESCRITA: Atualizar produto completo
    internal const string Update = @"
            UPDATE Products
            SET
                Name = @Name,
                Price = @Price,
                Stock = @Stock
            WHERE Id = @Id";

    // ESCRITA: Atualizar apenas o preço (operação comum)
    internal const string UpdatePrice = @"
            UPDATE Products
            SET Price = @Price
            WHERE Id = @Id";

    // ESCRITA: Deletar produto
    internal const string Delete = @"
            DELETE FROM Products
            WHERE Id = @Id";

    // AGREGAÇÃO: Contar total de produtos
    internal const string CountTotal = @"
            SELECT COUNT(*)
            FROM Products";

    // AGREGAÇÃO: Calcular valor total do estoque
    internal const string TotalStockValue = @"
            SELECT SUM(Price * Stock)
            FROM Products";

    // Query otimizada: não traz dados, apenas verifica existência
    internal const string Exists = @"
            SELECT COUNT(1)
            FROM Products
            WHERE Id = @Id";

    internal const string GetWithCategories = @"
        SELECT
            p.Id, p.Name, p.Price, p.Stock,
            c.Id, c.Name
        FROM Products p
        INNER JOIN Categories c ON p.CategoryId = c.Id";
}