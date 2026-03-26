using CoachManagement_Api.Entity;
using CoachManagement_Api.Repositories.interfaces;
using MySqlConnector;

namespace CoachManagement_Api.Repositories;

public class ClubsLocalitesRepository : IClubsLocalitesRepository
{
    private readonly string _connectionString;
    public ClubsLocalitesRepository(IConfiguration configuration) => _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

    public async Task<IReadOnlyList<ClubsLocalitesA>> GetByUserIdAsync(int userId)
    {
        await using var c = new MySqlConnection(_connectionString); await c.OpenAsync();
        const string sql = "SELECT cla.id_localites, cla.id_clubs, cla.created_at, cla.updated_at FROM ClubsLocalites_a cla INNER JOIN Clubs c ON c.id_clubs = cla.id_clubs WHERE c.fk_users_id=@userId ORDER BY cla.id_clubs, cla.id_localites";
        await using var cmd = new MySqlCommand(sql,c); cmd.Parameters.AddWithValue("@userId", userId);
        var list = new List<ClubsLocalitesA>(); await using var r = await cmd.ExecuteReaderAsync(); while(await r.ReadAsync()) list.Add(Map(r)); return list;
    }

    public async Task<ClubsLocalitesA?> GetByIdAsync(int idLocalite, int idClub, int userId)
    {
        await using var c = new MySqlConnection(_connectionString); await c.OpenAsync();
        const string sql = "SELECT cla.id_localites, cla.id_clubs, cla.created_at, cla.updated_at FROM ClubsLocalites_a cla INNER JOIN Clubs c ON c.id_clubs = cla.id_clubs WHERE cla.id_localites=@idLocalite AND cla.id_clubs=@idClub AND c.fk_users_id=@userId LIMIT 1";
        await using var cmd = new MySqlCommand(sql,c); cmd.Parameters.AddWithValue("@idLocalite", idLocalite); cmd.Parameters.AddWithValue("@idClub", idClub); cmd.Parameters.AddWithValue("@userId", userId);
        await using var r = await cmd.ExecuteReaderAsync(); return await r.ReadAsync()?Map(r):null;
    }

    public async Task<ClubsLocalitesA?> CreateAsync(ClubsLocalitesA e, int userId)
    {
        await using var c = new MySqlConnection(_connectionString); await c.OpenAsync();
        const string sql = "INSERT INTO ClubsLocalites_a (id_localites, id_clubs) SELECT @idLocalite, @idClub FROM Clubs c WHERE c.id_clubs=@idClub AND c.fk_users_id=@userId LIMIT 1";
        await using var cmd = new MySqlCommand(sql,c); cmd.Parameters.AddWithValue("@idLocalite", e.id_localites); cmd.Parameters.AddWithValue("@idClub", e.id_clubs); cmd.Parameters.AddWithValue("@userId", userId);
        var rows = await cmd.ExecuteNonQueryAsync(); if (rows == 0) return null;
        return await GetByIdAsync(e.id_localites, e.id_clubs, userId);
    }

    public async Task<bool> DeleteAsync(int idLocalite, int idClub, int userId)
    {
        await using var c = new MySqlConnection(_connectionString); await c.OpenAsync();
        const string sql = "DELETE cla FROM ClubsLocalites_a cla INNER JOIN Clubs c ON c.id_clubs = cla.id_clubs WHERE cla.id_localites=@idLocalite AND cla.id_clubs=@idClub AND c.fk_users_id=@userId";
        await using var cmd = new MySqlCommand(sql,c); cmd.Parameters.AddWithValue("@idLocalite", idLocalite); cmd.Parameters.AddWithValue("@idClub", idClub); cmd.Parameters.AddWithValue("@userId", userId);
        return await cmd.ExecuteNonQueryAsync() > 0;
    }

    private static ClubsLocalitesA Map(MySqlDataReader r)=>new(){id_localites=r.GetInt32("id_localites"),id_clubs=r.GetInt32("id_clubs"),created_at=r.GetDateTime("created_at"),updated_at=r.GetDateTime("updated_at")};
}
