using CoachManagement_Api.Entity;
using CoachManagement_Api.Repositories.interfaces;
using MySqlConnector;

namespace CoachManagement_Api.Repositories;

public class LeagueRepository : ILeagueRepository
{
    private readonly string _connectionString;

    public LeagueRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

    public async Task<IReadOnlyList<League>> GetAllAsync()
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        const string sql = "SELECT id_leagues, name FROM Leagues ORDER BY name";
        await using var cmd = new MySqlCommand(sql, connection);

        var list = new List<League>();
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(new League
            {
                id_leagues = reader.GetInt32("id_leagues"),
                name = reader.GetString("name")
            });
        }
        return list;
    }

    public async Task<League?> GetByIdAsync(int id)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        const string sql = "SELECT id_leagues, name FROM Leagues WHERE id_leagues = @id LIMIT 1";
        await using var cmd = new MySqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@id", id);

        await using var reader = await cmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
            return null;

        return new League
        {
            id_leagues = reader.GetInt32("id_leagues"),
            name = reader.GetString("name")
        };
    }
}
