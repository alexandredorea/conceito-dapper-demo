using Conceito.Dapper.Demo.Api.Domain.Interfaces;
using Conceito.Dapper.Demo.Api.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// 1. Registrar a factory de conexão como Singleton
// Singleton = uma única instância para toda aplicação
builder.Services.AddSingleton<IDbConnectionFactory, SqlConnectionFactory>();

// 2. Registrar o repositório como Scoped
// Scoped = uma instância por requisição HTTP
builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();