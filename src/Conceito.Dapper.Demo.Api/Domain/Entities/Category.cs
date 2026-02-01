namespace Conceito.Dapper.Demo.Api.Domain.Entities;

public sealed class Category
{
    public int Id { get; set; }
    public string Nome { get; init; } = string.Empty;
}