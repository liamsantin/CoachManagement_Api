using CoachManagement_Api.Entity;
using CoachManagement_Api.Repositories.interfaces;
using MySqlConnector;

namespace CoachManagement_Api.Repositories;

public class ReplacementRepository : IReplacementRepository
{
    private readonly string _connectionString;
    public ReplacementRepository(IConfiguration configuration)=>_connectionString=configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

    public async Task<IReadOnlyList<Replacement>> GetByMatchIdAsync(int matchId, int userId)
    {
        await using var c = new MySqlConnection(_connectionString); await c.OpenAsync();
        const string sql=@"SELECT r.id_replacements,r.minute,r.fk_matchs_id,r.fk_play_entering,r.fk_play_outgoing,r.created_at,r.updated_at
FROM Replacements r
INNER JOIN Matchs m ON m.id_events=r.fk_matchs_id
INNER JOIN Teams t ON t.id_teams=m.fk_teams_id AND t.fk_users_id=@u
WHERE r.fk_matchs_id=@m ORDER BY r.minute";
        await using var cmd = new MySqlCommand(sql,c); cmd.Parameters.AddWithValue("@u",userId); cmd.Parameters.AddWithValue("@m",matchId);
        var l = new List<Replacement>(); await using var r = await cmd.ExecuteReaderAsync(); while(await r.ReadAsync()) l.Add(Map(r)); return l;
    }

    public async Task<Replacement?> GetByIdAsync(int id, int userId)
    {
        await using var c = new MySqlConnection(_connectionString); await c.OpenAsync();
        const string sql=@"SELECT r.id_replacements,r.minute,r.fk_matchs_id,r.fk_play_entering,r.fk_play_outgoing,r.created_at,r.updated_at
FROM Replacements r
INNER JOIN Matchs m ON m.id_events=r.fk_matchs_id
INNER JOIN Teams t ON t.id_teams=m.fk_teams_id AND t.fk_users_id=@u
WHERE r.id_replacements=@id LIMIT 1";
        await using var cmd = new MySqlCommand(sql,c); cmd.Parameters.AddWithValue("@u",userId); cmd.Parameters.AddWithValue("@id",id);
        await using var r = await cmd.ExecuteReaderAsync(); return await r.ReadAsync()?Map(r):null;
    }

    public async Task<Replacement?> CreateAsync(Replacement e, int userId)
    {
        await using var c = new MySqlConnection(_connectionString); await c.OpenAsync();
        const string sql=@"INSERT INTO Replacements (minute,fk_matchs_id,fk_play_entering,fk_play_outgoing)
SELECT @mi,@m,@in,@out
FROM Matchs m INNER JOIN Teams t ON t.id_teams=m.fk_teams_id AND t.fk_users_id=@u
INNER JOIN Players pi ON pi.id_players=@in AND pi.fk_teams_id=m.fk_teams_id
INNER JOIN Players po ON po.id_players=@out AND po.fk_teams_id=m.fk_teams_id
WHERE m.id_events=@m LIMIT 1;
SELECT LAST_INSERT_ID();";
        await using var cmd = new MySqlCommand(sql,c); cmd.Parameters.AddWithValue("@mi",e.minute); cmd.Parameters.AddWithValue("@m",e.fk_matchs_id); cmd.Parameters.AddWithValue("@in",e.fk_play_entering); cmd.Parameters.AddWithValue("@out",e.fk_play_outgoing); cmd.Parameters.AddWithValue("@u",userId);
        var idObj = await cmd.ExecuteScalarAsync(); if(idObj==null||idObj==DBNull.Value) return null; return await GetByIdAsync(Convert.ToInt32(idObj), userId);
    }

    public async Task<bool> UpdateAsync(Replacement e, int userId)
    {
        await using var c = new MySqlConnection(_connectionString); await c.OpenAsync();
        const string sql=@"UPDATE Replacements r
INNER JOIN Matchs m ON m.id_events=r.fk_matchs_id
INNER JOIN Teams t ON t.id_teams=m.fk_teams_id AND t.fk_users_id=@u
SET r.minute=@mi,r.fk_play_entering=@in,r.fk_play_outgoing=@out
WHERE r.id_replacements=@id";
        await using var cmd = new MySqlCommand(sql,c); cmd.Parameters.AddWithValue("@u",userId); cmd.Parameters.AddWithValue("@id",e.id_replacements); cmd.Parameters.AddWithValue("@mi",e.minute); cmd.Parameters.AddWithValue("@in",e.fk_play_entering); cmd.Parameters.AddWithValue("@out",e.fk_play_outgoing);
        return await cmd.ExecuteNonQueryAsync()>0;
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        await using var c = new MySqlConnection(_connectionString); await c.OpenAsync();
        const string sql=@"DELETE r FROM Replacements r
INNER JOIN Matchs m ON m.id_events=r.fk_matchs_id
INNER JOIN Teams t ON t.id_teams=m.fk_teams_id AND t.fk_users_id=@u
WHERE r.id_replacements=@id";
        await using var cmd = new MySqlCommand(sql,c); cmd.Parameters.AddWithValue("@u",userId); cmd.Parameters.AddWithValue("@id",id);
        return await cmd.ExecuteNonQueryAsync()>0;
    }

    private static Replacement Map(MySqlDataReader r)=>new(){id_replacements=r.GetInt32("id_replacements"),minute=r.GetDecimal("minute"),fk_matchs_id=r.GetInt32("fk_matchs_id"),fk_play_entering=r.GetInt32("fk_play_entering"),fk_play_outgoing=r.GetInt32("fk_play_outgoing"),created_at=r.GetDateTime("created_at"),updated_at=r.GetDateTime("updated_at")};
}
