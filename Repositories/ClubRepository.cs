using CoachManagement_Api.Entity;
using CoachManagement_Api.Repositories.interfaces;
using MySqlConnector;

namespace CoachManagement_Api.Repositories;

public class ClubRepository : IClubRepository
{
    private readonly string _connectionString;

    public ClubRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

    public async Task<IReadOnlyList<Club>> GetAllByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sql = """
            SELECT id_clubs, fk_users_id, name, created_at, updated_at
            FROM Clubs
            WHERE fk_users_id = @userId
            ORDER BY name
            """;

        await using var cmd = new MySqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@userId", userId);

        var list = new List<Club>();
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
            list.Add(MapClub(reader));
        return list;
    }

    public async Task<Club?> GetByIdAndUserIdAsync(int clubId, int userId, CancellationToken cancellationToken = default)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sql = """
            SELECT id_clubs, fk_users_id, name, created_at, updated_at
            FROM Clubs
            WHERE id_clubs = @id AND fk_users_id = @userId
            LIMIT 1
            """;

        await using var cmd = new MySqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@id", clubId);
        cmd.Parameters.AddWithValue("@userId", userId);

        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        if (await reader.ReadAsync(cancellationToken))
            return MapClub(reader);
        return null;
    }

    public async Task<int> CreateAsync(Club club, CancellationToken cancellationToken = default)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sql = """
            INSERT INTO Clubs (fk_users_id, name)
            VALUES (@fk_users_id, @name);
            SELECT LAST_INSERT_ID();
            """;

        await using var cmd = new MySqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@fk_users_id", club.fk_users_id);
        cmd.Parameters.AddWithValue("@name", club.name);

        var result = await cmd.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt32(result);
    }

    public async Task<bool> UpdateAsync(Club club, int userId, CancellationToken cancellationToken = default)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sql = """
            UPDATE Clubs
            SET name = @name
            WHERE id_clubs = @id_clubs AND fk_users_id = @userId
            """;

        await using var cmd = new MySqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@name", club.name);
        cmd.Parameters.AddWithValue("@id_clubs", club.id_clubs);
        cmd.Parameters.AddWithValue("@userId", userId);

        var rows = await cmd.ExecuteNonQueryAsync(cancellationToken);
        return rows > 0;
    }

    public async Task<bool> DeleteAsync(int clubId, int userId, CancellationToken cancellationToken = default)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sql = """
            DELETE FROM Clubs
            WHERE id_clubs = @id AND fk_users_id = @userId
            """;

        await using var cmd = new MySqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@id", clubId);
        cmd.Parameters.AddWithValue("@userId", userId);

        var rows = await cmd.ExecuteNonQueryAsync(cancellationToken);
        return rows > 0;
    }

    private static Club MapClub(MySqlDataReader reader)
    {
        return new Club
        {
            id_clubs = reader.GetInt32("id_clubs"),
            fk_users_id = reader.GetInt32("fk_users_id"),
            name = reader.GetString("name"),
            created_at = reader.GetDateTime("created_at"),
            updated_at = reader.GetDateTime("updated_at")
        };
    }
}
