using CoachManagement_Api.Entity;
using CoachManagement_Api.Repositories.interfaces;
using MySqlConnector;

namespace CoachManagement_Api.Repositories;

public class CantonRepository : ICantonRepository
{
    private readonly string _connectionString;

    public CantonRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

    public async Task<IReadOnlyList<Canton>> GetAllAsync()
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        const string sql = """
            SELECT id_cantons, code, name
            FROM Cantons
            ORDER BY code
            """;

        await using var cmd = new MySqlCommand(sql, connection);
        var list = new List<Canton>();
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            list.Add(MapCanton(reader));
        return list;
    }

    public async Task<Canton?> GetByIdAsync(int id)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        const string sql = """
            SELECT id_cantons, code, name
            FROM Cantons
            WHERE id_cantons = @id
            LIMIT 1
            """;

        await using var cmd = new MySqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@id", id);

        await using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
            return MapCanton(reader);
        return null;
    }

    private static Canton MapCanton(MySqlDataReader reader)
    {
        return new Canton
        {
            id_cantons = reader.GetInt32("id_cantons"),
            code = reader.GetString("code"),
            name = reader.GetString("name")
        };
    }
}
