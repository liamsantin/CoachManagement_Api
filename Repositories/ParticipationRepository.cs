using CoachManagement_Api.Entity;
using CoachManagement_Api.Repositories.interfaces;
using MySqlConnector;

namespace CoachManagement_Api.Repositories;

public class ParticipationRepository : IParticipationRepository
{
    private readonly string _connectionString;
    public ParticipationRepository(IConfiguration configuration) => _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

    public async Task<IReadOnlyList<Participation>> GetByMatchIdAsync(int matchId, int userId)
    {
        await using var c = new MySqlConnection(_connectionString); await c.OpenAsync();
        const string sql = @"SELECT pa.id_players,pa.id_matchs,pa.noteOn10,pa.notes,pa.tempsJeu,pa.but,pa.passeD,pa.cartonJaune,pa.cartonRouge,pa.created_at,pa.updated_at
FROM Participations_a pa
INNER JOIN Matchs m ON m.id_events=pa.id_matchs
INNER JOIN Teams t ON t.id_teams=m.fk_teams_id AND t.fk_users_id=@userId
WHERE pa.id_matchs=@matchId ORDER BY pa.id_players";
        await using var cmd = new MySqlCommand(sql,c); cmd.Parameters.AddWithValue("@matchId", matchId); cmd.Parameters.AddWithValue("@userId", userId);
        var l = new List<Participation>(); await using var r = await cmd.ExecuteReaderAsync(); while(await r.ReadAsync()) l.Add(Map(r)); return l;
    }

    public async Task<Participation?> GetByIdAsync(int playerId, int matchId, int userId)
    {
        await using var c = new MySqlConnection(_connectionString); await c.OpenAsync();
        const string sql = @"SELECT pa.id_players,pa.id_matchs,pa.noteOn10,pa.notes,pa.tempsJeu,pa.but,pa.passeD,pa.cartonJaune,pa.cartonRouge,pa.created_at,pa.updated_at
FROM Participations_a pa
INNER JOIN Matchs m ON m.id_events=pa.id_matchs
INNER JOIN Teams t ON t.id_teams=m.fk_teams_id AND t.fk_users_id=@userId
WHERE pa.id_players=@playerId AND pa.id_matchs=@matchId LIMIT 1";
        await using var cmd = new MySqlCommand(sql,c); cmd.Parameters.AddWithValue("@playerId", playerId); cmd.Parameters.AddWithValue("@matchId", matchId); cmd.Parameters.AddWithValue("@userId", userId);
        await using var r = await cmd.ExecuteReaderAsync(); return await r.ReadAsync()?Map(r):null;
    }

    public async Task<Participation?> CreateAsync(Participation e, int userId)
    {
        await using var c = new MySqlConnection(_connectionString); await c.OpenAsync();
        const string sql = @"INSERT INTO Participations_a (id_players,id_matchs,noteOn10,notes,tempsJeu,but,passeD,cartonJaune,cartonRouge)
SELECT @p,@m,@n,@no,@t,@b,@pd,@cj,@cr
FROM Matchs m INNER JOIN Teams tm ON tm.id_teams=m.fk_teams_id AND tm.fk_users_id=@userId
INNER JOIN Players pl ON pl.id_players=@p AND pl.fk_teams_id=m.fk_teams_id
WHERE m.id_events=@m LIMIT 1";
        await using var cmd = new MySqlCommand(sql,c);
        cmd.Parameters.AddWithValue("@userId", userId); cmd.Parameters.AddWithValue("@p", e.id_players); cmd.Parameters.AddWithValue("@m", e.id_matchs);
        cmd.Parameters.AddWithValue("@n", string.IsNullOrWhiteSpace(e.noteOn10)?DBNull.Value:e.noteOn10!); cmd.Parameters.AddWithValue("@no", string.IsNullOrWhiteSpace(e.notes)?DBNull.Value:e.notes!); cmd.Parameters.AddWithValue("@t", e.tempsJeu ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@b", e.but); cmd.Parameters.AddWithValue("@pd", e.passeD); cmd.Parameters.AddWithValue("@cj", e.cartonJaune); cmd.Parameters.AddWithValue("@cr", e.cartonRouge);
        if (await cmd.ExecuteNonQueryAsync()==0) return null;
        return await GetByIdAsync(e.id_players, e.id_matchs, userId);
    }

    public async Task<bool> UpdateAsync(Participation e, int userId)
    {
        await using var c = new MySqlConnection(_connectionString); await c.OpenAsync();
        const string sql = @"UPDATE Participations_a pa
INNER JOIN Matchs m ON m.id_events=pa.id_matchs
INNER JOIN Teams tm ON tm.id_teams=m.fk_teams_id AND tm.fk_users_id=@userId
SET pa.noteOn10=@n,pa.notes=@no,pa.tempsJeu=@t,pa.but=@b,pa.passeD=@pd,pa.cartonJaune=@cj,pa.cartonRouge=@cr
WHERE pa.id_players=@p AND pa.id_matchs=@m";
        await using var cmd = new MySqlCommand(sql,c);
        cmd.Parameters.AddWithValue("@userId", userId); cmd.Parameters.AddWithValue("@p", e.id_players); cmd.Parameters.AddWithValue("@m", e.id_matchs);
        cmd.Parameters.AddWithValue("@n", string.IsNullOrWhiteSpace(e.noteOn10)?DBNull.Value:e.noteOn10!); cmd.Parameters.AddWithValue("@no", string.IsNullOrWhiteSpace(e.notes)?DBNull.Value:e.notes!); cmd.Parameters.AddWithValue("@t", e.tempsJeu ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@b", e.but); cmd.Parameters.AddWithValue("@pd", e.passeD); cmd.Parameters.AddWithValue("@cj", e.cartonJaune); cmd.Parameters.AddWithValue("@cr", e.cartonRouge);
        return await cmd.ExecuteNonQueryAsync()>0;
    }

    public async Task<bool> DeleteAsync(int playerId, int matchId, int userId)
    {
        await using var c = new MySqlConnection(_connectionString); await c.OpenAsync();
        const string sql = @"DELETE pa FROM Participations_a pa
INNER JOIN Matchs m ON m.id_events=pa.id_matchs
INNER JOIN Teams tm ON tm.id_teams=m.fk_teams_id AND tm.fk_users_id=@userId
WHERE pa.id_players=@p AND pa.id_matchs=@m";
        await using var cmd = new MySqlCommand(sql,c); cmd.Parameters.AddWithValue("@p", playerId); cmd.Parameters.AddWithValue("@m", matchId); cmd.Parameters.AddWithValue("@userId", userId);
        return await cmd.ExecuteNonQueryAsync()>0;
    }

    private static Participation Map(MySqlDataReader r)=>new(){id_players=r.GetInt32("id_players"),id_matchs=r.GetInt32("id_matchs"),noteOn10=r.IsDBNull(r.GetOrdinal("noteOn10"))?null:r.GetString("noteOn10"),notes=r.IsDBNull(r.GetOrdinal("notes"))?null:r.GetString("notes"),tempsJeu=r.IsDBNull(r.GetOrdinal("tempsJeu"))?null:r.GetDecimal("tempsJeu"),but=r.GetInt32("but"),passeD=r.GetInt32("passeD"),cartonJaune=r.GetInt32("cartonJaune"),cartonRouge=r.GetInt32("cartonRouge"),created_at=r.GetDateTime("created_at"),updated_at=r.GetDateTime("updated_at")};
}
