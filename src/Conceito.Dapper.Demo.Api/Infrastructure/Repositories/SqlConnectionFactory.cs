using Conceito.Dapper.Demo.Api.Domain.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Conceito.Dapper.Demo.Api.Infrastructure.Repositories;

/// <summary>
/// Implementação para SQL Server
/// </summary>
internal sealed class SqlConnectionFactory(IConfiguration configuration) : IDbConnectionFactory
{
    private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' não encontrada");

    public IDbConnection CreateConnection()
    {
        // Cria uma nova conexão com SQL Server
        return new SqlConnection(_connectionString);
    }
}