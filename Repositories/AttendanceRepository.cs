using CoachManagement_Api.DTOs.Attendance;
using CoachManagement_Api.Entity;
using CoachManagement_Api.Repositories.interfaces;
using MySqlConnector;

namespace CoachManagement_Api.Repositories;

public class AttendanceRepository : IAttendanceRepository
{
    private readonly string _connectionString;

    public AttendanceRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

    public async Task<IReadOnlyList<Attendance>> GetByTrainingIdAsync(int trainingId)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        // L'autorisation (training appartenant à l'utilisateur) est faite dans AttendanceService via ITrainingRepository.
        const string sql = """
            SELECT
                aa.id_players,
                aa.id_trainings,
                aa.notes,
                aa.retard,
                aa.motif,
                aa.created_at,
                aa.updated_at
            FROM Attendances_a aa
            WHERE aa.id_trainings = @trainingId
            ORDER BY aa.id_players
            """;

        await using var cmd = new MySqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@trainingId", trainingId);

        var list = new List<Attendance>();
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            list.Add(MapAttendance(reader));
        return list;
    }

    public async Task<Attendance?> GetByIdAsync(int playerId, int trainingId, int userId)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        const string sql = """
            SELECT
                aa.id_players,
                aa.id_trainings,
                aa.notes,
                aa.retard,
                aa.motif,
                aa.created_at,
                aa.updated_at
            FROM Attendances_a aa
            INNER JOIN Trainings t ON t.id_events = aa.id_trainings
            INNER JOIN Events e ON e.id_events = t.id_events
            INNER JOIN Teams tm ON tm.id_teams = e.fk_teams_id AND tm.fk_users_id = @userId
            INNER JOIN Players p ON p.id_players = aa.id_players AND p.fk_teams_id = e.fk_teams_id
            WHERE aa.id_players = @playerId AND aa.id_trainings = @trainingId
            LIMIT 1
            """;

        await using var cmd = new MySqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@playerId", playerId);
        cmd.Parameters.AddWithValue("@trainingId", trainingId);
        cmd.Parameters.AddWithValue("@userId", userId);

        await using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
            return MapAttendance(reader);
        return null;
    }

    public async Task<Attendance?> CreateAsync(Attendance attendance, int userId)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        const string sql = """
            INSERT INTO Attendances_a (id_players, id_trainings, notes, retard, motif)
            SELECT
                p.id_players,
                t.id_events,
                @notes,
                @retard,
                @motif
            FROM Trainings t
            INNER JOIN Events e ON e.id_events = t.id_events
            INNER JOIN Teams tm ON tm.id_teams = e.fk_teams_id AND tm.fk_users_id = @userId
            INNER JOIN Players p ON p.id_players = @id_players AND p.fk_teams_id = e.fk_teams_id
            WHERE t.id_events = @id_trainings
            LIMIT 1
            """;

        await using var cmd = new MySqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@userId", userId);
        cmd.Parameters.AddWithValue("@id_players", attendance.id_players);
        cmd.Parameters.AddWithValue("@id_trainings", attendance.id_trainings);
        cmd.Parameters.AddWithValue("@notes", string.IsNullOrWhiteSpace(attendance.notes) ? DBNull.Value : attendance.notes!);
        cmd.Parameters.AddWithValue("@retard", attendance.retard);
        cmd.Parameters.AddWithValue("@motif", string.IsNullOrWhiteSpace(attendance.motif) ? DBNull.Value : attendance.motif!);

        var rows = await cmd.ExecuteNonQueryAsync();
        if (rows == 0)
            return null;

        return await GetByIdAsync(attendance.id_players, attendance.id_trainings, userId);
    }

    public async Task<bool> UpdateAsync(Attendance attendance, int userId)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        const string sql = """
            UPDATE Attendances_a aa
            INNER JOIN Trainings t ON t.id_events = aa.id_trainings
            INNER JOIN Events e ON e.id_events = t.id_events
            INNER JOIN Teams tm ON tm.id_teams = e.fk_teams_id AND tm.fk_users_id = @userId
            INNER JOIN Players p ON p.id_players = aa.id_players AND p.fk_teams_id = e.fk_teams_id
            SET
                aa.notes = @notes,
                aa.retard = @retard,
                aa.motif = @motif
            WHERE aa.id_players = @id_players AND aa.id_trainings = @id_trainings
            """;

        await using var cmd = new MySqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@userId", userId);
        cmd.Parameters.AddWithValue("@id_players", attendance.id_players);
        cmd.Parameters.AddWithValue("@id_trainings", attendance.id_trainings);
        cmd.Parameters.AddWithValue("@notes", string.IsNullOrWhiteSpace(attendance.notes) ? DBNull.Value : attendance.notes!);
        cmd.Parameters.AddWithValue("@retard", attendance.retard);
        cmd.Parameters.AddWithValue("@motif", string.IsNullOrWhiteSpace(attendance.motif) ? DBNull.Value : attendance.motif!);

        var rows = await cmd.ExecuteNonQueryAsync();
        return rows > 0;
    }

    public async Task<bool> DeleteAsync(int playerId, int trainingId, int userId)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        const string sql = """
            DELETE aa
            FROM Attendances_a aa
            INNER JOIN Trainings t ON t.id_events = aa.id_trainings
            INNER JOIN Events e ON e.id_events = t.id_events
            INNER JOIN Teams tm ON tm.id_teams = e.fk_teams_id AND tm.fk_users_id = @userId
            WHERE aa.id_players = @playerId AND aa.id_trainings = @trainingId
            """;

        await using var cmd = new MySqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@playerId", playerId);
        cmd.Parameters.AddWithValue("@trainingId", trainingId);
        cmd.Parameters.AddWithValue("@userId", userId);

        var rows = await cmd.ExecuteNonQueryAsync();
        return rows > 0;
    }

    private static Attendance MapAttendance(MySqlDataReader reader)
    {
        return new Attendance
        {
            id_players = reader.GetInt32("id_players"),
            id_trainings = reader.GetInt32("id_trainings"),
            notes = reader.IsDBNull(reader.GetOrdinal("notes")) ? null : reader.GetString("notes"),
            retard = reader.GetBoolean("retard"),
            motif = reader.IsDBNull(reader.GetOrdinal("motif")) ? null : reader.GetString("motif"),
            created_at = reader.GetDateTime("created_at"),
            updated_at = reader.GetDateTime("updated_at")
        };
    }
}
