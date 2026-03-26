using CoachManagement_Api.Entity;
using CoachManagement_Api.Repositories.interfaces;
using MySqlConnector;

namespace CoachManagement_Api.Repositories;

public class OpponentRepository : IOpponentRepository
{
    private readonly string _connectionString;
    public OpponentRepository(IConfiguration configuration) => _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

    public async Task<IReadOnlyList<Opponent>> GetAllAsync()
    {
        await using var c = new MySqlConnection(_connectionString); await c.OpenAsync();
        const string sql = "SELECT id_opponents,fk_leagues_id,name,club,created_at,updated_at FROM Opponents ORDER BY name";
        await using var cmd = new MySqlCommand(sql, c);
        var list = new List<Opponent>(); await using var r = await cmd.ExecuteReaderAsync();
        while (await r.ReadAsync()) list.Add(Map(r));
        return list;
    }
    public async Task<Opponent?> GetByIdAsync(int id)
    {
        await using var c = new MySqlConnection(_connectionString); await c.OpenAsync();
        await using var cmd = new MySqlCommand("SELECT id_opponents,fk_leagues_id,name,club,created_at,updated_at FROM Opponents WHERE id_opponents=@id LIMIT 1", c);
        cmd.Parameters.AddWithValue("@id", id);
        await using var r = await cmd.ExecuteReaderAsync(); return await r.ReadAsync() ? Map(r) : null;
    }
    public async Task<int> CreateAsync(Opponent e)
    {
        await using var c = new MySqlConnection(_connectionString); await c.OpenAsync();
        await using var cmd = new MySqlCommand("INSERT INTO Opponents (fk_leagues_id,name,club) VALUES (@fk,@n,@c); SELECT LAST_INSERT_ID();", c);
        cmd.Parameters.AddWithValue("@fk", e.fk_leagues_id); cmd.Parameters.AddWithValue("@n", e.name); cmd.Parameters.AddWithValue("@c", string.IsNullOrWhiteSpace(e.club) ? DBNull.Value : e.club!);
        return Convert.ToInt32(await cmd.ExecuteScalarAsync());
    }
    public async Task<bool> UpdateAsync(Opponent e)
    {
        await using var c = new MySqlConnection(_connectionString); await c.OpenAsync();
        await using var cmd = new MySqlCommand("UPDATE Opponents SET fk_leagues_id=@fk,name=@n,club=@c WHERE id_opponents=@id", c);
        cmd.Parameters.AddWithValue("@id", e.id_opponents); cmd.Parameters.AddWithValue("@fk", e.fk_leagues_id); cmd.Parameters.AddWithValue("@n", e.name); cmd.Parameters.AddWithValue("@c", string.IsNullOrWhiteSpace(e.club) ? DBNull.Value : e.club!);
        return await cmd.ExecuteNonQueryAsync() > 0;
    }
    public async Task<bool> DeleteAsync(int id)
    {
        await using var c = new MySqlConnection(_connectionString); await c.OpenAsync();
        await using var cmd = new MySqlCommand("DELETE FROM Opponents WHERE id_opponents=@id", c); cmd.Parameters.AddWithValue("@id", id);
        return await cmd.ExecuteNonQueryAsync() > 0;
    }
    private static Opponent Map(MySqlDataReader r)=>new(){id_opponents=r.GetInt32("id_opponents"),fk_leagues_id=r.GetInt32("fk_leagues_id"),name=r.GetString("name"),club=r.IsDBNull(r.GetOrdinal("club"))?null:r.GetString("club"),created_at=r.GetDateTime("created_at"),updated_at=r.GetDateTime("updated_at")};
}
