using CoachManagement_Api.Entity;
using CoachManagement_Api.Repositories.interfaces;
using MySqlConnector;

namespace CoachManagement_Api.Repositories;

public class TypesTrainingRepository : ITypesTrainingRepository
{
    private readonly string _connectionString;
    public TypesTrainingRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }
    public async Task<IReadOnlyList<TypesTraining>> GetAllAsync()
    {
        await using var c = new MySqlConnection(_connectionString); await c.OpenAsync();
        await using var cmd = new MySqlCommand("SELECT id_typesTraining, type FROM TypesTraining ORDER BY id_typesTraining", c);
        var list = new List<TypesTraining>(); await using var r = await cmd.ExecuteReaderAsync();
        while (await r.ReadAsync()) list.Add(new TypesTraining { id_typesTraining = r.GetInt32("id_typesTraining"), type = r.GetString("type") });
        return list;
    }
    public async Task<TypesTraining?> GetByIdAsync(int id)
    {
        await using var c = new MySqlConnection(_connectionString); await c.OpenAsync();
        await using var cmd = new MySqlCommand("SELECT id_typesTraining, type FROM TypesTraining WHERE id_typesTraining=@id LIMIT 1", c); cmd.Parameters.AddWithValue("@id", id);
        await using var r = await cmd.ExecuteReaderAsync(); if (!await r.ReadAsync()) return null;
        return new TypesTraining { id_typesTraining = r.GetInt32("id_typesTraining"), type = r.GetString("type") };
    }
}
