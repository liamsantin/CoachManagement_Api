using CoachManagement_Api.Entity;
using CoachManagement_Api.Repositories.interfaces;
using MySqlConnector;

namespace CoachManagement_Api.Repositories;

public class PlayersLineupRepository : IPlayersLineupRepository
{
    private readonly string _connectionString;
    public PlayersLineupRepository(IConfiguration configuration)=>_connectionString=configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

    public async Task<IReadOnlyList<PlayersLineup>> GetByLineupIdAsync(int lineupId, int userId)
    {
        await using var c=new MySqlConnection(_connectionString); await c.OpenAsync();
        const string sql=@"SELECT pl.id_playersLineup,pl.fk_players_id,pl.fk_lineup_id,pl.fk_positions_id,pl.titulaire,pl.numMaillot,pl.capitaine,pl.created_at,pl.updated_at
FROM PlayersLineup pl
INNER JOIN Lineup l ON l.id_lineup=pl.fk_lineup_id
INNER JOIN Matchs m ON m.id_events=l.fk_matchs_id
INNER JOIN Teams t ON t.id_teams=m.fk_teams_id AND t.fk_users_id=@u
WHERE pl.fk_lineup_id=@id";
        await using var cmd=new MySqlCommand(sql,c); cmd.Parameters.AddWithValue("@id",lineupId); cmd.Parameters.AddWithValue("@u",userId);
        var list=new List<PlayersLineup>(); await using var r=await cmd.ExecuteReaderAsync(); while(await r.ReadAsync()) list.Add(Map(r)); return list;
    }

    public async Task<PlayersLineup?> GetByIdAsync(int idPlayersLineup, int userId)
    {
        await using var c=new MySqlConnection(_connectionString); await c.OpenAsync();
        const string sql=@"SELECT pl.id_playersLineup,pl.fk_players_id,pl.fk_lineup_id,pl.fk_positions_id,pl.titulaire,pl.numMaillot,pl.capitaine,pl.created_at,pl.updated_at
FROM PlayersLineup pl
INNER JOIN Lineup l ON l.id_lineup=pl.fk_lineup_id
INNER JOIN Matchs m ON m.id_events=l.fk_matchs_id
INNER JOIN Teams t ON t.id_teams=m.fk_teams_id AND t.fk_users_id=@u
WHERE pl.id_playersLineup=@id LIMIT 1";
        await using var cmd=new MySqlCommand(sql,c); cmd.Parameters.AddWithValue("@id",idPlayersLineup); cmd.Parameters.AddWithValue("@u",userId);
        await using var r=await cmd.ExecuteReaderAsync(); return await r.ReadAsync()?Map(r):null;
    }

    public async Task<PlayersLineup?> CreateAsync(PlayersLineup e, int userId)
    {
        await using var c=new MySqlConnection(_connectionString); await c.OpenAsync();
        const string sql=@"INSERT INTO PlayersLineup (fk_players_id,fk_lineup_id,fk_positions_id,titulaire,numMaillot,capitaine)
SELECT @p,@l,@pos,@ti,@num,@cap
FROM Lineup l
INNER JOIN Matchs m ON m.id_events=l.fk_matchs_id
INNER JOIN Teams t ON t.id_teams=m.fk_teams_id AND t.fk_users_id=@u
INNER JOIN Players p ON p.id_players=@p AND p.fk_teams_id=m.fk_teams_id
WHERE l.id_lineup=@l LIMIT 1;
SELECT LAST_INSERT_ID();";
        await using var cmd=new MySqlCommand(sql,c); cmd.Parameters.AddWithValue("@p",e.fk_players_id); cmd.Parameters.AddWithValue("@l",e.fk_lineup_id); cmd.Parameters.AddWithValue("@pos",e.fk_positions_id); cmd.Parameters.AddWithValue("@ti",e.titulaire); cmd.Parameters.AddWithValue("@num",e.numMaillot ?? (object)DBNull.Value); cmd.Parameters.AddWithValue("@cap",e.capitaine); cmd.Parameters.AddWithValue("@u",userId);
        var idObj=await cmd.ExecuteScalarAsync(); if(idObj==null||idObj==DBNull.Value) return null; return await GetByIdAsync(Convert.ToInt32(idObj), userId);
    }

    public async Task<bool> UpdateAsync(PlayersLineup e, int userId)
    {
        await using var c=new MySqlConnection(_connectionString); await c.OpenAsync();
        const string sql=@"UPDATE PlayersLineup pl
INNER JOIN Lineup l ON l.id_lineup=pl.fk_lineup_id
INNER JOIN Matchs m ON m.id_events=l.fk_matchs_id
INNER JOIN Teams t ON t.id_teams=m.fk_teams_id AND t.fk_users_id=@u
SET pl.fk_positions_id=@pos,pl.titulaire=@ti,pl.numMaillot=@num,pl.capitaine=@cap
WHERE pl.id_playersLineup=@id";
        await using var cmd=new MySqlCommand(sql,c); cmd.Parameters.AddWithValue("@id",e.id_playersLineup); cmd.Parameters.AddWithValue("@pos",e.fk_positions_id); cmd.Parameters.AddWithValue("@ti",e.titulaire); cmd.Parameters.AddWithValue("@num",e.numMaillot ?? (object)DBNull.Value); cmd.Parameters.AddWithValue("@cap",e.capitaine); cmd.Parameters.AddWithValue("@u",userId);
        return await cmd.ExecuteNonQueryAsync()>0;
    }

    public async Task<bool> DeleteAsync(int idPlayersLineup, int userId)
    {
        await using var c=new MySqlConnection(_connectionString); await c.OpenAsync();
        const string sql=@"DELETE pl FROM PlayersLineup pl
INNER JOIN Lineup l ON l.id_lineup=pl.fk_lineup_id
INNER JOIN Matchs m ON m.id_events=l.fk_matchs_id
INNER JOIN Teams t ON t.id_teams=m.fk_teams_id AND t.fk_users_id=@u
WHERE pl.id_playersLineup=@id";
        await using var cmd=new MySqlCommand(sql,c); cmd.Parameters.AddWithValue("@id",idPlayersLineup); cmd.Parameters.AddWithValue("@u",userId);
        return await cmd.ExecuteNonQueryAsync()>0;
    }

    private static PlayersLineup Map(MySqlDataReader r)=>new(){id_playersLineup=r.GetInt32("id_playersLineup"),fk_players_id=r.GetInt32("fk_players_id"),fk_lineup_id=r.GetInt32("fk_lineup_id"),fk_positions_id=r.GetInt32("fk_positions_id"),titulaire=r.GetBoolean("titulaire"),numMaillot=r.IsDBNull(r.GetOrdinal("numMaillot"))?null:r.GetInt32("numMaillot"),capitaine=r.GetBoolean("capitaine"),created_at=r.GetDateTime("created_at"),updated_at=r.GetDateTime("updated_at")};
}
