using CoachManagement_Api.Entity;
using CoachManagement_Api.Repositories.interfaces;
using MySqlConnector;

namespace CoachManagement_Api.Repositories;

public class LocaliteRepository : ILocaliteRepository
{
    private readonly string _connectionString;

    public LocaliteRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

    public async Task<IReadOnlyList<Localite>> GetAllAsync()
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        const string sql = """
            SELECT id_localites, fk_cantons_id, npa, localite
            FROM Localites
            ORDER BY npa, localite
            """;

        await using var cmd = new MySqlCommand(sql, connection);
        var list = new List<Localite>();
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            list.Add(MapLocalite(reader));
        return list;
    }

    public async Task<Localite?> GetByIdAsync(int id)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        const string sql = """
            SELECT id_localites, fk_cantons_id, npa, localite
            FROM Localites
            WHERE id_localites = @id
            LIMIT 1
            """;

        await using var cmd = new MySqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@id", id);

        await using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
            return MapLocalite(reader);
        return null;
    }

    private static Localite MapLocalite(MySqlDataReader reader)
    {
        return new Localite
        {
            id_localites = reader.GetInt32("id_localites"),
            fk_cantons_id = reader.GetInt32("fk_cantons_id"),
            npa = reader.GetString("npa"),
            localite = reader.GetString("localite")
        };
    }
}
