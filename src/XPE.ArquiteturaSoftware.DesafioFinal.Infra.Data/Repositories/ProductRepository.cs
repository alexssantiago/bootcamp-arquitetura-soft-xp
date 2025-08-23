using Dapper;
using MySqlConnector;
using System.Data;
using XPE.ArquiteturaSoftware.DesafioFinal.Domain.Models;
using XPE.ArquiteturaSoftware.DesafioFinal.Infra.Data.Interfaces;
using XPE.ArquiteturaSoftware.DesafioFinal.Infra.Data.Queries;

namespace XPE.ArquiteturaSoftware.DesafioFinal.Infra.Data.Repositories;

public sealed class ProductRepository(IDbConnection connection) : IProductRepository
{
    public async Task<int> CreateAsync(Product model)
    {
        var parameters = new DynamicParameters();
        parameters.Add("Name", model.Name, DbType.String);
        parameters.Add("Description", model.Description, DbType.String);
        parameters.Add("Price", model.Price, DbType.Decimal);
        parameters.Add("Active", model.Active, DbType.Boolean);

        if (connection.State != ConnectionState.Open)
            await ((MySqlConnection)connection).OpenAsync();

        var id = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(ProductQueries.Create, parameters));

        return id;
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        var parameters = new DynamicParameters();
        parameters.Add("Id", id, DbType.Int32);

        if (connection.State != ConnectionState.Open)
            await ((MySqlConnection)connection).OpenAsync();

        return await connection.QueryFirstOrDefaultAsync<Product>(
            new CommandDefinition(ProductQueries.GetById, parameters));
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        if (connection.State != ConnectionState.Open)
            await ((MySqlConnection)connection).OpenAsync();

        return await connection.QueryAsync<Product>(
            new CommandDefinition(ProductQueries.GetAll));
    }

    public async Task<IEnumerable<Product>> FindByNameAsync(string name)
    {
        var parameters = new DynamicParameters();
        parameters.Add("Pattern", $"%{name}%", DbType.String);

        if (connection.State != ConnectionState.Open)
            await ((MySqlConnection)connection).OpenAsync();

        return await connection.QueryAsync<Product>(
            new CommandDefinition(ProductQueries.FindByName, parameters));
    }

    public async Task<int> CountAsync()
    {
        if (connection.State != ConnectionState.Open)
            await ((MySqlConnection)connection).OpenAsync();

        return await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(ProductQueries.Count));
    }

    public async Task<bool> UpdateAsync(int id, Product updated)
    {
        var parameters = new DynamicParameters();
        parameters.Add("Id", id, DbType.Int32);
        parameters.Add("Name", updated.Name, DbType.String);
        parameters.Add("Description", updated.Description, DbType.String);
        parameters.Add("Price", updated.Price, DbType.Decimal);
        parameters.Add("Active", updated.Active, DbType.Boolean);

        if (connection.State != ConnectionState.Open)
            await ((MySqlConnection)connection).OpenAsync();

        var rows = await connection.ExecuteAsync(
            new CommandDefinition(ProductQueries.Update, parameters));

        return rows > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var parameters = new DynamicParameters();
        parameters.Add("Id", id, DbType.Int32);

        if (connection.State != ConnectionState.Open)
            await ((MySqlConnection)connection).OpenAsync();

        var rows = await connection.ExecuteAsync(
            new CommandDefinition(ProductQueries.Delete, parameters));

        return rows > 0;
    }
}