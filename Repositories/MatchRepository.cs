using CoachManagement_Api.DTOs.Match;
using CoachManagement_Api.Repositories.interfaces;
using MySqlConnector;

namespace CoachManagement_Api.Repositories;

public class MatchRepository : IMatchRepository
{
    private readonly string _connectionString;
    public MatchRepository(IConfiguration configuration) => _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

    public async Task<IReadOnlyList<MatchResponse>> GetAllByUserIdAndTeamIdAsync(int userId, int teamId)
    {
        await using var c = new MySqlConnection(_connectionString); await c.OpenAsync();
        const string sql = @"SELECT m.id_events,e.fk_teams_id,m.fk_opponents_id,m.fk_status_id,m.fk_types_id,m.fk_seasons_id,m.fk_lineup_id,l.fk_formations_id,l.name AS lineup_name,l.notes AS lineup_notes,m.fk_localites_id,m.scoreEquipe,m.scoreOpponent,m.temps,m.stade,m.arbitre,e.date,e.name,e.startDate,e.endDate,e.created_at AS event_created_at,e.updated_at AS event_updated_at,m.created_at AS match_created_at,m.updated_at AS match_updated_at
FROM Matchs m
INNER JOIN Events e ON e.id_events=m.id_events
INNER JOIN Teams t ON t.id_teams=e.fk_teams_id
INNER JOIN Lineup l ON l.id_lineup=m.fk_lineup_id
WHERE t.fk_users_id=@userId AND e.fk_teams_id=@teamId ORDER BY e.startDate DESC";
        await using var cmd = new MySqlCommand(sql, c); cmd.Parameters.AddWithValue("@userId", userId); cmd.Parameters.AddWithValue("@teamId", teamId);
        var list = new List<MatchResponse>(); await using var r = await cmd.ExecuteReaderAsync(); while(await r.ReadAsync()) list.Add(Map(r)); return list;
    }

    public async Task<MatchResponse?> GetByIdAndUserIdAsync(int idEvents, int userId)
    {
        await using var c = new MySqlConnection(_connectionString); await c.OpenAsync();
        const string sql = @"SELECT m.id_events,e.fk_teams_id,m.fk_opponents_id,m.fk_status_id,m.fk_types_id,m.fk_seasons_id,m.fk_lineup_id,l.fk_formations_id,l.name AS lineup_name,l.notes AS lineup_notes,m.fk_localites_id,m.scoreEquipe,m.scoreOpponent,m.temps,m.stade,m.arbitre,e.date,e.name,e.startDate,e.endDate,e.created_at AS event_created_at,e.updated_at AS event_updated_at,m.created_at AS match_created_at,m.updated_at AS match_updated_at
FROM Matchs m
INNER JOIN Events e ON e.id_events=m.id_events
INNER JOIN Teams t ON t.id_teams=e.fk_teams_id
INNER JOIN Lineup l ON l.id_lineup=m.fk_lineup_id
WHERE m.id_events=@id AND t.fk_users_id=@userId LIMIT 1";
        await using var cmd = new MySqlCommand(sql,c); cmd.Parameters.AddWithValue("@id", idEvents); cmd.Parameters.AddWithValue("@userId", userId);
        await using var r = await cmd.ExecuteReaderAsync(); return await r.ReadAsync()?Map(r):null;
    }

    public async Task<MatchResponse?> CreateAsync(int userId, MatchCreateRequest request)
    {
        await using var c = new MySqlConnection(_connectionString); await c.OpenAsync(); await using var tx = await c.BeginTransactionAsync();
        try
        {
            if (!await TeamOwnedAsync(c, tx, request.fk_teams_id, userId)) { await tx.RollbackAsync(); return null; }

            int lineupId;
            await using (var cmdL = new MySqlCommand("INSERT INTO Lineup (fk_matchs_id,fk_formations_id,name,notes) VALUES (NULL,@f,@n,@no); SELECT LAST_INSERT_ID();", c, tx))
            {
                cmdL.Parameters.AddWithValue("@f", request.fk_formations_id); cmdL.Parameters.AddWithValue("@n", request.lineup_name); cmdL.Parameters.AddWithValue("@no", string.IsNullOrWhiteSpace(request.lineup_notes)?DBNull.Value:request.lineup_notes!);
                lineupId = Convert.ToInt32(await cmdL.ExecuteScalarAsync());
            }

            int idEvents;
            await using (var cmdE = new MySqlCommand("INSERT INTO Events (fk_teams_id,date,name,startDate,endDate) VALUES (@t,@d,@n,@s,@e); SELECT LAST_INSERT_ID();", c, tx))
            {
                cmdE.Parameters.AddWithValue("@t", request.fk_teams_id); cmdE.Parameters.AddWithValue("@d", request.date.ToString("yyyy-MM-dd")); cmdE.Parameters.AddWithValue("@n", request.name ?? (object)DBNull.Value); cmdE.Parameters.AddWithValue("@s", request.startDate); cmdE.Parameters.AddWithValue("@e", request.endDate);
                idEvents = Convert.ToInt32(await cmdE.ExecuteScalarAsync());
            }

            await using (var cmdM = new MySqlCommand("INSERT INTO Matchs (id_events,fk_teams_id,fk_opponents_id,fk_status_id,fk_types_id,fk_seasons_id,fk_lineup_id,fk_localites_id,scoreEquipe,scoreOpponent,temps,stade,arbitre) VALUES (@id,@t,@op,@st,@ty,@se,@li,@loc,@sq,@so,@tm,@sta,@arb)", c, tx))
            {
                cmdM.Parameters.AddWithValue("@id", idEvents); cmdM.Parameters.AddWithValue("@t", request.fk_teams_id); cmdM.Parameters.AddWithValue("@op", request.fk_opponents_id); cmdM.Parameters.AddWithValue("@st", request.fk_status_id); cmdM.Parameters.AddWithValue("@ty", request.fk_types_id); cmdM.Parameters.AddWithValue("@se", request.fk_seasons_id); cmdM.Parameters.AddWithValue("@li", lineupId); cmdM.Parameters.AddWithValue("@loc", request.fk_localites_id);
                cmdM.Parameters.AddWithValue("@sq", request.scoreEquipe ?? (object)DBNull.Value); cmdM.Parameters.AddWithValue("@so", request.scoreOpponent ?? (object)DBNull.Value); cmdM.Parameters.AddWithValue("@tm", request.temps ?? (object)DBNull.Value); cmdM.Parameters.AddWithValue("@sta", string.IsNullOrWhiteSpace(request.stade)?DBNull.Value:request.stade!); cmdM.Parameters.AddWithValue("@arb", string.IsNullOrWhiteSpace(request.arbitre)?DBNull.Value:request.arbitre!);
                await cmdM.ExecuteNonQueryAsync();
            }

            await using (var cmdLU = new MySqlCommand("UPDATE Lineup SET fk_matchs_id=@id WHERE id_lineup=@lineup", c, tx))
            {
                cmdLU.Parameters.AddWithValue("@id", idEvents); cmdLU.Parameters.AddWithValue("@lineup", lineupId); await cmdLU.ExecuteNonQueryAsync();
            }

            await tx.CommitAsync();
            return await GetByIdAndUserIdAsync(idEvents, userId);
        }
        catch { await tx.RollbackAsync(); throw; }
    }

    public async Task<MatchResponse?> UpdateAsync(int idEvents, int userId, MatchUpdateRequest request)
    {
        var existing = await GetByIdAndUserIdAsync(idEvents, userId); if (existing == null) return null;
        await using var c = new MySqlConnection(_connectionString); await c.OpenAsync(); await using var tx = await c.BeginTransactionAsync();
        try
        {
            if (!await TeamOwnedAsync(c, tx, request.fk_teams_id, userId)) { await tx.RollbackAsync(); return null; }

            await using (var cmdE = new MySqlCommand("UPDATE Events SET fk_teams_id=@t,date=@d,name=@n,startDate=@s,endDate=@e WHERE id_events=@id", c, tx))
            {
                cmdE.Parameters.AddWithValue("@id", idEvents); cmdE.Parameters.AddWithValue("@t", request.fk_teams_id); cmdE.Parameters.AddWithValue("@d", request.date.ToString("yyyy-MM-dd")); cmdE.Parameters.AddWithValue("@n", request.name ?? (object)DBNull.Value); cmdE.Parameters.AddWithValue("@s", request.startDate); cmdE.Parameters.AddWithValue("@e", request.endDate);
                if (await cmdE.ExecuteNonQueryAsync()==0){await tx.RollbackAsync(); return null;}
            }

            await using (var cmdM = new MySqlCommand("UPDATE Matchs SET fk_teams_id=@t,fk_opponents_id=@op,fk_status_id=@st,fk_types_id=@ty,fk_seasons_id=@se,fk_localites_id=@loc,scoreEquipe=@sq,scoreOpponent=@so,temps=@tm,stade=@sta,arbitre=@arb WHERE id_events=@id", c, tx))
            {
                cmdM.Parameters.AddWithValue("@id", idEvents); cmdM.Parameters.AddWithValue("@t", request.fk_teams_id); cmdM.Parameters.AddWithValue("@op", request.fk_opponents_id); cmdM.Parameters.AddWithValue("@st", request.fk_status_id); cmdM.Parameters.AddWithValue("@ty", request.fk_types_id); cmdM.Parameters.AddWithValue("@se", request.fk_seasons_id); cmdM.Parameters.AddWithValue("@loc", request.fk_localites_id);
                cmdM.Parameters.AddWithValue("@sq", request.scoreEquipe ?? (object)DBNull.Value); cmdM.Parameters.AddWithValue("@so", request.scoreOpponent ?? (object)DBNull.Value); cmdM.Parameters.AddWithValue("@tm", request.temps ?? (object)DBNull.Value); cmdM.Parameters.AddWithValue("@sta", string.IsNullOrWhiteSpace(request.stade)?DBNull.Value:request.stade!); cmdM.Parameters.AddWithValue("@arb", string.IsNullOrWhiteSpace(request.arbitre)?DBNull.Value:request.arbitre!);
                await cmdM.ExecuteNonQueryAsync();
            }

            await using (var cmdL = new MySqlCommand("UPDATE Lineup SET fk_formations_id=@f,name=@n,notes=@no WHERE id_lineup=@id", c, tx))
            {
                cmdL.Parameters.AddWithValue("@id", existing.fk_lineup_id); cmdL.Parameters.AddWithValue("@f", request.fk_formations_id); cmdL.Parameters.AddWithValue("@n", request.lineup_name); cmdL.Parameters.AddWithValue("@no", string.IsNullOrWhiteSpace(request.lineup_notes)?DBNull.Value:request.lineup_notes!);
                await cmdL.ExecuteNonQueryAsync();
            }

            await tx.CommitAsync();
            return await GetByIdAndUserIdAsync(idEvents, userId);
        }
        catch { await tx.RollbackAsync(); throw; }
    }

    public async Task<bool> DeleteAsync(int idEvents, int userId)
    {
        var existing = await GetByIdAndUserIdAsync(idEvents, userId); if (existing == null) return false;
        await using var c = new MySqlConnection(_connectionString); await c.OpenAsync(); await using var tx = await c.BeginTransactionAsync();
        try
        {
            await using (var cmdR = new MySqlCommand("DELETE FROM Replacements WHERE fk_matchs_id=@id", c, tx)) { cmdR.Parameters.AddWithValue("@id", idEvents); await cmdR.ExecuteNonQueryAsync(); }
            await using (var cmdP = new MySqlCommand("DELETE FROM Participations_a WHERE id_matchs=@id", c, tx)) { cmdP.Parameters.AddWithValue("@id", idEvents); await cmdP.ExecuteNonQueryAsync(); }
            await using (var cmdLU = new MySqlCommand("UPDATE Lineup SET fk_matchs_id=NULL WHERE id_lineup=@lineup", c, tx)) { cmdLU.Parameters.AddWithValue("@lineup", existing.fk_lineup_id); await cmdLU.ExecuteNonQueryAsync(); }
            await using (var cmdM = new MySqlCommand("DELETE FROM Matchs WHERE id_events=@id", c, tx)) { cmdM.Parameters.AddWithValue("@id", idEvents); if (await cmdM.ExecuteNonQueryAsync()==0){await tx.RollbackAsync(); return false;} }
            await using (var cmdE = new MySqlCommand("DELETE FROM Events WHERE id_events=@id", c, tx)) { cmdE.Parameters.AddWithValue("@id", idEvents); await cmdE.ExecuteNonQueryAsync(); }
            await tx.CommitAsync(); return true;
        }
        catch { await tx.RollbackAsync(); throw; }
    }

    private static MatchResponse Map(MySqlDataReader r) => new()
    {
        id_events = r.GetInt32("id_events"),
        fk_teams_id = r.GetInt32("fk_teams_id"),
        fk_opponents_id = r.GetInt32("fk_opponents_id"),
        fk_status_id = r.GetInt32("fk_status_id"),
        fk_types_id = r.GetInt32("fk_types_id"),
        fk_seasons_id = r.GetInt32("fk_seasons_id"),
        fk_lineup_id = r.GetInt32("fk_lineup_id"),
        fk_formations_id = r.GetInt32("fk_formations_id"),
        lineup_name = r.GetString("lineup_name"),
        lineup_notes = r.IsDBNull(r.GetOrdinal("lineup_notes")) ? null : r.GetString("lineup_notes"),
        fk_localites_id = r.GetInt32("fk_localites_id"),
        scoreEquipe = r.IsDBNull(r.GetOrdinal("scoreEquipe")) ? null : r.GetInt32("scoreEquipe"),
        scoreOpponent = r.IsDBNull(r.GetOrdinal("scoreOpponent")) ? null : r.GetInt32("scoreOpponent"),
        temps = r.IsDBNull(r.GetOrdinal("temps")) ? null : r.GetDecimal("temps"),
        stade = r.IsDBNull(r.GetOrdinal("stade")) ? null : r.GetString("stade"),
        arbitre = r.IsDBNull(r.GetOrdinal("arbitre")) ? null : r.GetString("arbitre"),
        date = DateOnly.FromDateTime(r.GetDateTime("date")),
        name = r.IsDBNull(r.GetOrdinal("name")) ? null : r.GetString("name"),
        startDate = r.GetDateTime("startDate"),
        endDate = r.GetDateTime("endDate"),
        event_created_at = r.GetDateTime("event_created_at"),
        event_updated_at = r.GetDateTime("event_updated_at"),
        match_created_at = r.GetDateTime("match_created_at"),
        match_updated_at = r.GetDateTime("match_updated_at")
    };

    private static async Task<bool> TeamOwnedAsync(MySqlConnection c, MySqlTransaction tx, int teamId, int userId)
    {
        await using var cmd = new MySqlCommand("SELECT 1 FROM Teams WHERE id_teams=@id AND fk_users_id=@u LIMIT 1", c, tx);
        cmd.Parameters.AddWithValue("@id", teamId); cmd.Parameters.AddWithValue("@u", userId);
        var result = await cmd.ExecuteScalarAsync(); return result != null && result != DBNull.Value;
    }
}
