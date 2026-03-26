using CoachManagement_Api.Entity;
using CoachManagement_Api.Repositories.interfaces;
using MySqlConnector;

namespace CoachManagement_Api.Repositories;

public class PositionRepository : IPositionRepository
{
    private readonly string _connectionString;
    public PositionRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }
    public async Task<IReadOnlyList<Position>> GetAllAsync()
    {
        await using var c = new MySqlConnection(_connectionString); await c.OpenAsync();
        await using var cmd = new MySqlCommand("SELECT id_positions, code, description FROM Positions ORDER BY id_positions", c);
        var list = new List<Position>(); await using var r = await cmd.ExecuteReaderAsync();
        while (await r.ReadAsync()) list.Add(new Position { id_positions = r.GetInt32("id_positions"), code = r.IsDBNull(r.GetOrdinal("code")) ? null : r.GetString("code"), description = r.IsDBNull(r.GetOrdinal("description")) ? null : r.GetString("description") });
        return list;
    }
    public async Task<Position?> GetByIdAsync(int id)
    {
        await using var c = new MySqlConnection(_connectionString); await c.OpenAsync();
        await using var cmd = new MySqlCommand("SELECT id_positions, code, description FROM Positions WHERE id_positions=@id LIMIT 1", c); cmd.Parameters.AddWithValue("@id", id);
        await using var r = await cmd.ExecuteReaderAsync(); if (!await r.ReadAsync()) return null;
        return new Position { id_positions = r.GetInt32("id_positions"), code = r.IsDBNull(r.GetOrdinal("code")) ? null : r.GetString("code"), description = r.IsDBNull(r.GetOrdinal("description")) ? null : r.GetString("description") };
    }
}
