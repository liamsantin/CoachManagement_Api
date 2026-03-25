using CoachManagement_Api.Entity;
using CoachManagement_Api.Repositories.interfaces;
using MySqlConnector;

namespace CoachManagement_Api.Repositories;

public class TeamRepository : ITeamRepository
{
    private readonly string _connectionString;

    public TeamRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

    public async Task<IReadOnlyList<Team>> GetAllByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sql = """
            SELECT id_teams, fk_users_id, fk_clubs_id, fk_leagues_id, name, created_at, updated_at
            FROM Teams
            WHERE fk_users_id = @userId
            ORDER BY name
            """;

        await using var cmd = new MySqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@userId", userId);

        var list = new List<Team>();
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
            list.Add(MapTeam(reader));
        return list;
    }

    public async Task<Team?> GetByIdAndUserIdAsync(int teamId, int userId, CancellationToken cancellationToken = default)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sql = """
            SELECT id_teams, fk_users_id, fk_clubs_id, fk_leagues_id, name, created_at, updated_at
            FROM Teams
            WHERE id_teams = @id AND fk_users_id = @userId
            LIMIT 1
            """;

        await using var cmd = new MySqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@id", teamId);
        cmd.Parameters.AddWithValue("@userId", userId);

        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        if (await reader.ReadAsync(cancellationToken))
            return MapTeam(reader);
        return null;
    }

    public async Task<int> CreateAsync(Team team, CancellationToken cancellationToken = default)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sql = """
            INSERT INTO Teams (fk_users_id, fk_clubs_id, fk_leagues_id, name)
            VALUES (@fk_users_id, @fk_clubs_id, @fk_leagues_id, @name);
            SELECT LAST_INSERT_ID();
            """;

        await using var cmd = new MySqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@fk_users_id", team.fk_users_id);
        cmd.Parameters.AddWithValue("@fk_clubs_id", team.fk_clubs_id ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@fk_leagues_id", team.fk_leagues_id);
        cmd.Parameters.AddWithValue("@name", team.name);

        var result = await cmd.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt32(result);
    }

    public async Task<bool> UpdateAsync(Team team, int userId, CancellationToken cancellationToken = default)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sql = """
            UPDATE Teams
            SET fk_clubs_id = @fk_clubs_id,
                fk_leagues_id = @fk_leagues_id,
                name = @name
            WHERE id_teams = @id_teams AND fk_users_id = @userId
            """;

        await using var cmd = new MySqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@fk_clubs_id", team.fk_clubs_id ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@fk_leagues_id", team.fk_leagues_id);
        cmd.Parameters.AddWithValue("@name", team.name);
        cmd.Parameters.AddWithValue("@id_teams", team.id_teams);
        cmd.Parameters.AddWithValue("@userId", userId);

        var rows = await cmd.ExecuteNonQueryAsync(cancellationToken);
        return rows > 0;
    }

    public async Task<bool> DeleteAsync(int teamId, int userId, CancellationToken cancellationToken = default)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sql = """
            DELETE FROM Teams
            WHERE id_teams = @id AND fk_users_id = @userId
            """;

        await using var cmd = new MySqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@id", teamId);
        cmd.Parameters.AddWithValue("@userId", userId);

        var rows = await cmd.ExecuteNonQueryAsync(cancellationToken);
        return rows > 0;
    }

    private static Team MapTeam(MySqlDataReader reader)
    {
        return new Team
        {
            id_teams = reader.GetInt32("id_teams"),
            fk_users_id = reader.GetInt32("fk_users_id"),
            fk_clubs_id = reader.IsDBNull(reader.GetOrdinal("fk_clubs_id")) ? null : reader.GetInt32("fk_clubs_id"),
            fk_leagues_id = reader.GetInt32("fk_leagues_id"),
            name = reader.GetString("name"),
            created_at = reader.GetDateTime("created_at"),
            updated_at = reader.GetDateTime("updated_at")
        };
    }
}
