using CoachManagement_Api.DTOs.Training;
using CoachManagement_Api.Repositories.interfaces;
using MySqlConnector;

namespace CoachManagement_Api.Repositories;

public class TrainingRepository : ITrainingRepository
{
    private readonly string _connectionString;

    public TrainingRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

    public async Task<IReadOnlyList<TrainingResponse>> GetAllByUserIdAndTeamIdAsync(int userId, int teamId)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        const string sql = """
            SELECT
                e.id_events,
                e.date AS evt_date,
                e.name AS evt_name,
                e.startDate,
                e.endDate,
                e.fk_teams_id,
                t.fk_localites_id,
                t.fk_types_id,
                t.description,
                COALESCE(att.attendance_count, t.nbrPlayer) AS nbrPlayer,
                e.created_at AS evt_created_at,
                e.updated_at AS evt_updated_at,
                t.created_at AS tr_created_at,
                t.updated_at AS tr_updated_at
            FROM Trainings t
            INNER JOIN Events e ON e.id_events = t.id_events
            INNER JOIN Teams tm ON tm.id_teams = e.fk_teams_id
            LEFT JOIN (
                SELECT id_trainings, COUNT(*) AS attendance_count
                FROM Attendances_a
                GROUP BY id_trainings
            ) att ON att.id_trainings = t.id_events
            WHERE tm.fk_users_id = @userId
              AND e.fk_teams_id = @teamId
            ORDER BY e.startDate DESC
            """;

        await using var cmd = new MySqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@userId", userId);
        cmd.Parameters.AddWithValue("@teamId", teamId);

        var list = new List<TrainingResponse>();
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            list.Add(MapTrainingResponse(reader));
        return list;
    }

    public async Task<TrainingResponse?> GetByIdAndUserIdAsync(int idEvents, int userId)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        const string sql = """
            SELECT
                e.id_events,
                e.date AS evt_date,
                e.name AS evt_name,
                e.startDate,
                e.endDate,
                e.fk_teams_id,
                t.fk_localites_id,
                t.fk_types_id,
                t.description,
                COALESCE(att.attendance_count, t.nbrPlayer) AS nbrPlayer,
                e.created_at AS evt_created_at,
                e.updated_at AS evt_updated_at,
                t.created_at AS tr_created_at,
                t.updated_at AS tr_updated_at
            FROM Trainings t
            INNER JOIN Events e ON e.id_events = t.id_events
            INNER JOIN Teams tm ON tm.id_teams = e.fk_teams_id
            LEFT JOIN (
                SELECT id_trainings, COUNT(*) AS attendance_count
                FROM Attendances_a
                GROUP BY id_trainings
            ) att ON att.id_trainings = t.id_events
            WHERE t.id_events = @id AND tm.fk_users_id = @userId
            LIMIT 1
            """;

        await using var cmd = new MySqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@id", idEvents);
        cmd.Parameters.AddWithValue("@userId", userId);

        await using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
            return MapTrainingResponse(reader);
        return null;
    }

    public async Task<TrainingResponse?> CreateAsync(int userId, TrainingCreateRequest request)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();
        await using var tx = await connection.BeginTransactionAsync();

        try
        {
            if (!await TeamBelongsToUserAsync(connection, tx, request.fk_teams_id, userId))
            {
                await tx.RollbackAsync();
                return null;
            }

            // id_events sans AUTO_INCREMENT : on calcule le prochain id dans la transaction.
            // Préférez en base : ALTER TABLE Events MODIFY id_events INT NOT NULL AUTO_INCREMENT;
            int idEvents;
            await using (var cmdNext = new MySqlCommand(
                "SELECT COALESCE(MAX(id_events), 0) + 1 FROM Events",
                connection,
                tx))
            {
                var nextObj = await cmdNext.ExecuteScalarAsync();
                idEvents = Convert.ToInt32(nextObj);
            }

            const string insertEvent = """
                INSERT INTO Events (id_events, fk_teams_id, date, name, startDate, endDate)
                VALUES (@id_events, @fk_teams_id, @date, @name, @startDate, @endDate)
                """;

            await using (var cmdEvent = new MySqlCommand(insertEvent, connection, tx))
            {
                cmdEvent.Parameters.AddWithValue("@id_events", idEvents);
                cmdEvent.Parameters.AddWithValue("@fk_teams_id", request.fk_teams_id);
                cmdEvent.Parameters.AddWithValue("@date", request.date.ToString("yyyy-MM-dd"));
                cmdEvent.Parameters.AddWithValue("@name", request.name ?? (object)DBNull.Value);
                cmdEvent.Parameters.AddWithValue("@startDate", request.startDate);
                cmdEvent.Parameters.AddWithValue("@endDate", request.endDate);
                await cmdEvent.ExecuteNonQueryAsync();
            }

            const string insertTraining = """
                INSERT INTO Trainings (id_events, fk_localites_id, fk_types_id, description, nbrPlayer)
                VALUES (@id_events, @fk_localites_id, @fk_types_id, @description, @nbrPlayer);
                """;

            await using var cmdTr = new MySqlCommand(insertTraining, connection, tx);
            cmdTr.Parameters.AddWithValue("@id_events", idEvents);
            cmdTr.Parameters.AddWithValue("@fk_localites_id", request.fk_localites_id);
            cmdTr.Parameters.AddWithValue("@fk_types_id", request.fk_types_id ?? (object)DBNull.Value);
            cmdTr.Parameters.AddWithValue("@description", request.description ?? (object)DBNull.Value);
            cmdTr.Parameters.AddWithValue("@nbrPlayer", request.nbrPlayer);
            await cmdTr.ExecuteNonQueryAsync();

            await tx.CommitAsync();
            return await GetByIdAndUserIdAsync(idEvents, userId);
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }

    public async Task<TrainingResponse?> UpdateAsync(int idEvents, int userId, TrainingUpdateRequest request)
    {
        var existing = await GetByIdAndUserIdAsync(idEvents, userId);
        if (existing == null)
            return null;

        if (!await TeamBelongsToUserStandaloneAsync(request.fk_teams_id, userId))
            return null;

        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();
        await using var tx = await connection.BeginTransactionAsync();

        try
        {
            const string updEvent = """
                UPDATE Events
                SET fk_teams_id = @fk_teams_id,
                    date = @date,
                    name = @name,
                    startDate = @startDate,
                    endDate = @endDate
                WHERE id_events = @id_events
                """;

            await using (var cmdEvent = new MySqlCommand(updEvent, connection, tx))
            {
                cmdEvent.Parameters.AddWithValue("@fk_teams_id", request.fk_teams_id);
                cmdEvent.Parameters.AddWithValue("@date", request.date.ToString("yyyy-MM-dd"));
                cmdEvent.Parameters.AddWithValue("@name", request.name ?? (object)DBNull.Value);
                cmdEvent.Parameters.AddWithValue("@startDate", request.startDate);
                cmdEvent.Parameters.AddWithValue("@endDate", request.endDate);
                cmdEvent.Parameters.AddWithValue("@id_events", idEvents);
                var rowsE = await cmdEvent.ExecuteNonQueryAsync();
                if (rowsE == 0)
                {
                    await tx.RollbackAsync();
                    return null;
                }
            }

            const string updTr = """
                UPDATE Trainings
                SET fk_localites_id = @fk_localites_id,
                    fk_types_id = @fk_types_id,
                    description = @description,
                    nbrPlayer = @nbrPlayer
                WHERE id_events = @id_events
                """;

            await using (var cmdTr = new MySqlCommand(updTr, connection, tx))
            {
                cmdTr.Parameters.AddWithValue("@fk_localites_id", request.fk_localites_id);
                cmdTr.Parameters.AddWithValue("@fk_types_id", request.fk_types_id ?? (object)DBNull.Value);
                cmdTr.Parameters.AddWithValue("@description", request.description ?? (object)DBNull.Value);
                cmdTr.Parameters.AddWithValue("@nbrPlayer", request.nbrPlayer);
                cmdTr.Parameters.AddWithValue("@id_events", idEvents);
                await cmdTr.ExecuteNonQueryAsync();
            }

            // Keep attendances consistent if training team changes:
            // remove attendance rows for players outside the target team.
            const string cleanupAttendances = """
                DELETE aa
                FROM Attendances_a aa
                INNER JOIN Players p ON p.id_players = aa.id_players
                WHERE aa.id_trainings = @id_events
                  AND p.fk_teams_id <> @fk_teams_id
                """;

            await using (var cmdCleanup = new MySqlCommand(cleanupAttendances, connection, tx))
            {
                cmdCleanup.Parameters.AddWithValue("@id_events", idEvents);
                cmdCleanup.Parameters.AddWithValue("@fk_teams_id", request.fk_teams_id);
                await cmdCleanup.ExecuteNonQueryAsync();
            }

            await tx.CommitAsync();
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }

        return await GetByIdAndUserIdAsync(idEvents, userId);
    }

    private async Task<bool> TeamBelongsToUserStandaloneAsync(int teamId, int userId)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();
        return await TeamBelongsToUserAsync(connection, null, teamId, userId);
    }

    public async Task<bool> DeleteAsync(int idEvents, int userId)
    {
        var existing = await GetByIdAndUserIdAsync(idEvents, userId);
        if (existing == null)
            return false;

        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();
        await using var tx = await connection.BeginTransactionAsync();

        try
        {
            await using (var cmdAtt = new MySqlCommand(
                "DELETE FROM Attendances_a WHERE id_trainings = @id_events",
                connection, tx))
            {
                cmdAtt.Parameters.AddWithValue("@id_events", idEvents);
                await cmdAtt.ExecuteNonQueryAsync();
            }

            await using (var cmdTr = new MySqlCommand(
                "DELETE FROM Trainings WHERE id_events = @id_events",
                connection, tx))
            {
                cmdTr.Parameters.AddWithValue("@id_events", idEvents);
                var rowsT = await cmdTr.ExecuteNonQueryAsync();
                if (rowsT == 0)
                {
                    await tx.RollbackAsync();
                    return false;
                }
            }

            await using (var cmdEv = new MySqlCommand(
                "DELETE FROM Events WHERE id_events = @id_events",
                connection, tx))
            {
                cmdEv.Parameters.AddWithValue("@id_events", idEvents);
                await cmdEv.ExecuteNonQueryAsync();
            }

            await tx.CommitAsync();
            return true;
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }

    private static async Task<bool> TeamBelongsToUserAsync(MySqlConnection connection, MySqlTransaction? tx, int teamId, int userId)
    {
        const string sql = """
            SELECT 1 FROM Teams WHERE id_teams = @id AND fk_users_id = @userId LIMIT 1
            """;
        await using var cmd = tx is null
            ? new MySqlCommand(sql, connection)
            : new MySqlCommand(sql, connection, tx);
        cmd.Parameters.AddWithValue("@id", teamId);
        cmd.Parameters.AddWithValue("@userId", userId);
        var result = await cmd.ExecuteScalarAsync();
        return result != null && result != DBNull.Value;
    }

    private static TrainingResponse MapTrainingResponse(MySqlDataReader reader)
    {
        var dateVal = reader.GetDateTime("evt_date");
        return new TrainingResponse
        {
            id_events = reader.GetInt32("id_events"),
            date = DateOnly.FromDateTime(dateVal),
            name = reader.IsDBNull(reader.GetOrdinal("evt_name")) ? null : reader.GetString("evt_name"),
            startDate = reader.GetDateTime("startDate"),
            endDate = reader.GetDateTime("endDate"),
            fk_teams_id = reader.GetInt32("fk_teams_id"),
            fk_localites_id = reader.GetInt32("fk_localites_id"),
            fk_types_id = reader.IsDBNull(reader.GetOrdinal("fk_types_id")) ? null : reader.GetInt32("fk_types_id"),
            description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString("description"),
            nbrPlayer = reader.GetInt32("nbrPlayer"),
            event_created_at = reader.GetDateTime("evt_created_at"),
            event_updated_at = reader.GetDateTime("evt_updated_at"),
            training_created_at = reader.GetDateTime("tr_created_at"),
            training_updated_at = reader.GetDateTime("tr_updated_at")
        };
    }
}
