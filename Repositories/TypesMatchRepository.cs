using CoachManagement_Api.Entity;
using CoachManagement_Api.Repositories.interfaces;
using MySqlConnector;

namespace CoachManagement_Api.Repositories;

public class TypesMatchRepository : ITypesMatchRepository
{
    private readonly string _connectionString;
    public TypesMatchRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }
    public async Task<IReadOnlyList<TypesMatch>> GetAllAsync()
    {
        await using var c = new MySqlConnection(_connectionString); await c.OpenAsync();
        await using var cmd = new MySqlCommand("SELECT id_typesMatch, name FROM TypesMatch ORDER BY id_typesMatch", c);
        var list = new List<TypesMatch>(); await using var r = await cmd.ExecuteReaderAsync();
        while (await r.ReadAsync()) list.Add(new TypesMatch { id_typesMatch = r.GetInt32("id_typesMatch"), name = r.GetString("name") });
        return list;
    }
    public async Task<TypesMatch?> GetByIdAsync(int id)
    {
        await using var c = new MySqlConnection(_connectionString); await c.OpenAsync();
        await using var cmd = new MySqlCommand("SELECT id_typesMatch, name FROM TypesMatch WHERE id_typesMatch=@id LIMIT 1", c); cmd.Parameters.AddWithValue("@id", id);
        await using var r = await cmd.ExecuteReaderAsync(); if (!await r.ReadAsync()) return null;
        return new TypesMatch { id_typesMatch = r.GetInt32("id_typesMatch"), name = r.GetString("name") };
    }
}
