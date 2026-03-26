using CoachManagement_Api.Entity;
using CoachManagement_Api.Repositories.interfaces;
using MySqlConnector;

namespace CoachManagement_Api.Repositories;

public class StatusMatchRepository : IStatusMatchRepository
{
    private readonly string _connectionString;
    public StatusMatchRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }
    public async Task<IReadOnlyList<StatusMatch>> GetAllAsync()
    {
        await using var c = new MySqlConnection(_connectionString); await c.OpenAsync();
        await using var cmd = new MySqlCommand("SELECT id_statusMatch, name FROM StatusMatch ORDER BY id_statusMatch", c);
        var list = new List<StatusMatch>(); await using var r = await cmd.ExecuteReaderAsync();
        while (await r.ReadAsync()) list.Add(new StatusMatch { id_statusMatch = r.GetInt32("id_statusMatch"), name = r.GetString("name") });
        return list;
    }
    public async Task<StatusMatch?> GetByIdAsync(int id)
    {
        await using var c = new MySqlConnection(_connectionString); await c.OpenAsync();
        await using var cmd = new MySqlCommand("SELECT id_statusMatch, name FROM StatusMatch WHERE id_statusMatch=@id LIMIT 1", c); cmd.Parameters.AddWithValue("@id", id);
        await using var r = await cmd.ExecuteReaderAsync(); if (!await r.ReadAsync()) return null;
        return new StatusMatch { id_statusMatch = r.GetInt32("id_statusMatch"), name = r.GetString("name") };
    }
}
