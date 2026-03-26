using CoachManagement_Api.Entity;
using CoachManagement_Api.Repositories.interfaces;
using MySqlConnector;

namespace CoachManagement_Api.Repositories;

public class PlayerRepository : IPlayerRepository
{
    private readonly string _connectionString;

    public PlayerRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

    public async Task<IReadOnlyList<Player>> GetAllByUserIdAndTeamIdAsync(int userId, int teamId)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        const string sql = """
            SELECT
                p.id_players,
                p.fk_teams_id,
                p.fk_positions_id,
                p.nom,
                p.prenom,
                p.numeroMaillot,
                p.email,
                p.phone,
                p.anneeExp,
                p.poids,
                p.taille,
                p.dateNaiss,
                p.dateArrivee,
                p.photoUrl,
                p.created_at,
                p.updated_at
            FROM Players p
            INNER JOIN Teams tm ON tm.id_teams = p.fk_teams_id
            WHERE tm.fk_users_id = @userId
              AND p.fk_teams_id = @teamId
            ORDER BY p.nom, p.prenom
            """;

        await using var cmd = new MySqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@userId", userId);
        cmd.Parameters.AddWithValue("@teamId", teamId);

        var list = new List<Player>();
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            list.Add(MapPlayer(reader));
        return list;
    }

    public async Task<Player?> GetByIdAndUserIdAsync(int playerId, int userId)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        const string sql = """
            SELECT
                p.id_players,
                p.fk_teams_id,
                p.fk_positions_id,
                p.nom,
                p.prenom,
                p.numeroMaillot,
                p.email,
                p.phone,
                p.anneeExp,
                p.poids,
                p.taille,
                p.dateNaiss,
                p.dateArrivee,
                p.photoUrl,
                p.created_at,
                p.updated_at
            FROM Players p
            INNER JOIN Teams tm ON tm.id_teams = p.fk_teams_id
            WHERE p.id_players = @id AND tm.fk_users_id = @userId
            LIMIT 1
            """;

        await using var cmd = new MySqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@id", playerId);
        cmd.Parameters.AddWithValue("@userId", userId);

        await using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
            return MapPlayer(reader);
        return null;
    }

    public async Task<int?> CreateAsync(Player player, int userId)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        const string sql = """
            INSERT INTO Players (
                fk_teams_id, fk_positions_id, nom, prenom, numeroMaillot,
                email, phone, anneeExp, poids, taille, dateNaiss, dateArrivee, photoUrl
            )
            SELECT
                t.id_teams,
                @fk_positions_id,
                @nom,
                @prenom,
                @numeroMaillot,
                @email,
                @phone,
                @anneeExp,
                @poids,
                @taille,
                @dateNaiss,
                @dateArrivee,
                @photoUrl
            FROM Teams t
            WHERE t.id_teams = @fk_teams_id AND t.fk_users_id = @userId
            LIMIT 1
            """;

        await using (var cmd = new MySqlCommand(sql, connection))
        {
            AddPlayerParameters(cmd, player);
            cmd.Parameters.AddWithValue("@userId", userId);
            var rows = await cmd.ExecuteNonQueryAsync();
            if (rows == 0)
                return null;
        }

        await using var cmdId = new MySqlCommand("SELECT LAST_INSERT_ID();", connection);
        return Convert.ToInt32(await cmdId.ExecuteScalarAsync());
    }

    public async Task<bool> UpdateAsync(Player player, int userId)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        const string sql = """
            UPDATE Players p
            INNER JOIN Teams tm_old ON tm_old.id_teams = p.fk_teams_id AND tm_old.fk_users_id = @userId
            SET
                p.fk_teams_id = @fk_teams_id,
                p.fk_positions_id = @fk_positions_id,
                p.nom = @nom,
                p.prenom = @prenom,
                p.numeroMaillot = @numeroMaillot,
                p.email = @email,
                p.phone = @phone,
                p.anneeExp = @anneeExp,
                p.poids = @poids,
                p.taille = @taille,
                p.dateNaiss = @dateNaiss,
                p.dateArrivee = @dateArrivee,
                p.photoUrl = @photoUrl
            WHERE p.id_players = @id_players
            AND EXISTS (
                SELECT 1 FROM Teams t2
                WHERE t2.id_teams = @fk_teams_id AND t2.fk_users_id = @userId
            )
            """;

        await using var cmd = new MySqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@userId", userId);
        cmd.Parameters.AddWithValue("@id_players", player.id_players);
        cmd.Parameters.AddWithValue("@fk_teams_id", player.fk_teams_id);
        cmd.Parameters.AddWithValue("@fk_positions_id", player.fk_positions_id);
        cmd.Parameters.AddWithValue("@nom", player.nom);
        cmd.Parameters.AddWithValue("@prenom", player.prenom);
        cmd.Parameters.AddWithValue("@numeroMaillot", player.numeroMaillot ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@email", string.IsNullOrWhiteSpace(player.email) ? DBNull.Value : player.email!);
        cmd.Parameters.AddWithValue("@phone", string.IsNullOrWhiteSpace(player.phone) ? DBNull.Value : player.phone!);
        cmd.Parameters.AddWithValue("@anneeExp", player.anneeExp ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@poids", player.poids ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@taille", player.taille ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@dateNaiss", player.dateNaiss.HasValue ? player.dateNaiss.Value.ToString("yyyy-MM-dd") : DBNull.Value);
        cmd.Parameters.AddWithValue("@dateArrivee", player.dateArrivee.HasValue ? player.dateArrivee.Value.ToString("yyyy-MM-dd") : DBNull.Value);
        cmd.Parameters.AddWithValue("@photoUrl", string.IsNullOrWhiteSpace(player.photoUrl) ? DBNull.Value : player.photoUrl!);

        var rows = await cmd.ExecuteNonQueryAsync();
        return rows > 0;
    }

    public async Task<bool> DeleteAsync(int playerId, int userId)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        const string sql = """
            DELETE p FROM Players p
            INNER JOIN Teams tm ON tm.id_teams = p.fk_teams_id AND tm.fk_users_id = @userId
            WHERE p.id_players = @id
            """;

        await using var cmd = new MySqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@id", playerId);
        cmd.Parameters.AddWithValue("@userId", userId);

        var rows = await cmd.ExecuteNonQueryAsync();
        return rows > 0;
    }

    private static void AddPlayerParameters(MySqlCommand cmd, Player player)
    {
        cmd.Parameters.AddWithValue("@fk_teams_id", player.fk_teams_id);
        cmd.Parameters.AddWithValue("@fk_positions_id", player.fk_positions_id);
        cmd.Parameters.AddWithValue("@nom", player.nom);
        cmd.Parameters.AddWithValue("@prenom", player.prenom);
        cmd.Parameters.AddWithValue("@numeroMaillot", player.numeroMaillot ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@email", string.IsNullOrWhiteSpace(player.email) ? DBNull.Value : player.email!);
        cmd.Parameters.AddWithValue("@phone", string.IsNullOrWhiteSpace(player.phone) ? DBNull.Value : player.phone!);
        cmd.Parameters.AddWithValue("@anneeExp", player.anneeExp ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@poids", player.poids ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@taille", player.taille ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@dateNaiss", player.dateNaiss.HasValue ? player.dateNaiss.Value.ToString("yyyy-MM-dd") : DBNull.Value);
        cmd.Parameters.AddWithValue("@dateArrivee", player.dateArrivee.HasValue ? player.dateArrivee.Value.ToString("yyyy-MM-dd") : DBNull.Value);
        cmd.Parameters.AddWithValue("@photoUrl", string.IsNullOrWhiteSpace(player.photoUrl) ? DBNull.Value : player.photoUrl!);
    }

    private static Player MapPlayer(MySqlDataReader reader)
    {
        return new Player
        {
            id_players = reader.GetInt32("id_players"),
            fk_teams_id = reader.GetInt32("fk_teams_id"),
            fk_positions_id = reader.GetInt32("fk_positions_id"),
            nom = reader.GetString("nom"),
            prenom = reader.GetString("prenom"),
            numeroMaillot = reader.IsDBNull(reader.GetOrdinal("numeroMaillot")) ? null : reader.GetInt32("numeroMaillot"),
            email = reader.IsDBNull(reader.GetOrdinal("email")) ? null : reader.GetString("email"),
            phone = reader.IsDBNull(reader.GetOrdinal("phone")) ? null : reader.GetString("phone"),
            anneeExp = reader.IsDBNull(reader.GetOrdinal("anneeExp")) ? null : reader.GetInt32("anneeExp"),
            poids = reader.IsDBNull(reader.GetOrdinal("poids")) ? null : reader.GetDecimal("poids"),
            taille = reader.IsDBNull(reader.GetOrdinal("taille")) ? null : reader.GetDecimal("taille"),
            dateNaiss = reader.IsDBNull(reader.GetOrdinal("dateNaiss"))
                ? null
                : DateOnly.FromDateTime(reader.GetDateTime("dateNaiss")),
            dateArrivee = reader.IsDBNull(reader.GetOrdinal("dateArrivee"))
                ? null
                : DateOnly.FromDateTime(reader.GetDateTime("dateArrivee")),
            photoUrl = reader.IsDBNull(reader.GetOrdinal("photoUrl")) ? null : reader.GetString("photoUrl"),
            created_at = reader.GetDateTime("created_at"),
            updated_at = reader.GetDateTime("updated_at")
        };
    }
}
