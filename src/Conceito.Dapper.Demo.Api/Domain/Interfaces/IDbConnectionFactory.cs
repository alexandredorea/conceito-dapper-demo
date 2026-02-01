using System.Data;

namespace Conceito.Dapper.Demo.Api.Domain.Interfaces;

/// <summary>
/// Interface para criar conexões com o banco de dados
/// Facilita testes e troca de banco no futuro
/// </summary>
public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}