using CoachManagement_Api.Entity;
using CoachManagement_Api.Repositories.interfaces;
using MySqlConnector;

namespace CoachManagement_Api.Repositories;

public class LineupRepository : ILineupRepository
{
    private readonly string _connectionString;
    public LineupRepository(IConfiguration configuration) => _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

    public async Task<IReadOnlyList<Lineup>> GetByMatchIdAsync(int matchId, int userId)
    {
        await using var c = new MySqlConnection(_connectionString); await c.OpenAsync();
        const string sql = @"SELECT l.id_lineup,l.fk_matchs_id,l.fk_formations_id,l.name,l.notes,l.created_at,l.updated_at
FROM Lineup l
INNER JOIN Matchs m ON m.id_events=l.fk_matchs_id
INNER JOIN Teams t ON t.id_teams=m.fk_teams_id AND t.fk_users_id=@userId
WHERE l.fk_matchs_id=@matchId
ORDER BY l.id_lineup";
        await using var cmd = new MySqlCommand(sql,c); cmd.Parameters.AddWithValue("@matchId", matchId); cmd.Parameters.AddWithValue("@userId", userId);
        var list = new List<Lineup>(); await using var r = await cmd.ExecuteReaderAsync(); while(await r.ReadAsync()) list.Add(Map(r)); return list;
    }

    public async Task<Lineup?> GetByIdAsync(int lineupId, int userId)
    {
        await using var c = new MySqlConnection(_connectionString); await c.OpenAsync();
        const string sql = @"SELECT l.id_lineup,l.fk_matchs_id,l.fk_formations_id,l.name,l.notes,l.created_at,l.updated_at
FROM Lineup l
INNER JOIN Matchs m ON m.id_events=l.fk_matchs_id
INNER JOIN Teams t ON t.id_teams=m.fk_teams_id AND t.fk_users_id=@userId
WHERE l.id_lineup=@id
LIMIT 1";
        await using var cmd = new MySqlCommand(sql,c); cmd.Parameters.AddWithValue("@id", lineupId); cmd.Parameters.AddWithValue("@userId", userId);
        await using var r = await cmd.ExecuteReaderAsync(); return await r.ReadAsync() ? Map(r) : null;
    }

    public async Task<Lineup?> CreateAsync(Lineup lineup, int userId)
    {
        await using var c = new MySqlConnection(_connectionString); await c.OpenAsync();
        const string sql = @"INSERT INTO Lineup (fk_matchs_id,fk_formations_id,name,notes)
SELECT @matchId,@formationId,@name,@notes
FROM Matchs m
INNER JOIN Teams t ON t.id_teams=m.fk_teams_id AND t.fk_users_id=@userId
WHERE m.id_events=@matchId
LIMIT 1;
SELECT LAST_INSERT_ID();";
        await using var cmd = new MySqlCommand(sql,c);
        cmd.Parameters.AddWithValue("@matchId", lineup.fk_matchs_id ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@formationId", lineup.fk_formations_id);
        cmd.Parameters.AddWithValue("@name", lineup.name);
        cmd.Parameters.AddWithValue("@notes", string.IsNullOrWhiteSpace(lineup.notes) ? DBNull.Value : lineup.notes!);
        cmd.Parameters.AddWithValue("@userId", userId);

        var idObj = await cmd.ExecuteScalarAsync();
        if (idObj == null || idObj == DBNull.Value)
            return null;

        return await GetByIdAsync(Convert.ToInt32(idObj), userId);
    }

    public async Task<bool> UpdateAsync(Lineup lineup, int userId)
    {
        await using var c = new MySqlConnection(_connectionString); await c.OpenAsync();
        const string sql = @"UPDATE Lineup l
INNER JOIN Matchs m ON m.id_events=l.fk_matchs_id
INNER JOIN Teams t ON t.id_teams=m.fk_teams_id AND t.fk_users_id=@userId
SET l.fk_formations_id=@formationId,
    l.name=@name,
    l.notes=@notes
WHERE l.id_lineup=@id";
        await using var cmd = new MySqlCommand(sql,c);
        cmd.Parameters.AddWithValue("@id", lineup.id_lineup);
        cmd.Parameters.AddWithValue("@formationId", lineup.fk_formations_id);
        cmd.Parameters.AddWithValue("@name", lineup.name);
        cmd.Parameters.AddWithValue("@notes", string.IsNullOrWhiteSpace(lineup.notes) ? DBNull.Value : lineup.notes!);
        cmd.Parameters.AddWithValue("@userId", userId);
        return await cmd.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int lineupId, int userId)
    {
        await using var c = new MySqlConnection(_connectionString); await c.OpenAsync();
        const string sql = @"DELETE l FROM Lineup l
INNER JOIN Matchs m ON m.id_events=l.fk_matchs_id
INNER JOIN Teams t ON t.id_teams=m.fk_teams_id AND t.fk_users_id=@userId
WHERE l.id_lineup=@id";
        await using var cmd = new MySqlCommand(sql,c); cmd.Parameters.AddWithValue("@id", lineupId); cmd.Parameters.AddWithValue("@userId", userId);
        return await cmd.ExecuteNonQueryAsync() > 0;
    }

    private static Lineup Map(MySqlDataReader r) => new()
    {
        id_lineup = r.GetInt32("id_lineup"),
        fk_matchs_id = r.IsDBNull(r.GetOrdinal("fk_matchs_id")) ? null : r.GetInt32("fk_matchs_id"),
        fk_formations_id = r.GetInt32("fk_formations_id"),
        name = r.GetString("name"),
        notes = r.IsDBNull(r.GetOrdinal("notes")) ? null : r.GetString("notes"),
        created_at = r.GetDateTime("created_at"),
        updated_at = r.GetDateTime("updated_at")
    };
}
