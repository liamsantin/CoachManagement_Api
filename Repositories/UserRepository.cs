using CoachManagement_Api.Models;
using CoachManagement_Api.Repositories.interfaces;
using MySqlConnector;

namespace CoachManagement_Api.Repositories;

public class UserRepository : IUserRepository
{
    private readonly string _connectionString;

    public UserRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        const string sql = """
            SELECT id_users, username, password, email, phone, created_at, updated_at
            FROM Users
            WHERE id_users = @id
            LIMIT 1
            """;

        await using var cmd = new MySqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@id", id);

        await using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
            return MapUser(reader);
        return null;
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        const string sql = """
            SELECT id_users, username, password, email, phone, created_at, updated_at
            FROM Users
            WHERE username = @username
            LIMIT 1
            """;

        await using var cmd = new MySqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@username", username);

        await using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
            return MapUser(reader);
        return null;
    }

    public async Task<User?> GetByEmailAsync(string? email)
    {
        if (string.IsNullOrWhiteSpace(email)) return null;

        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        const string sql = """
            SELECT id_users, username, password, email, phone, created_at, updated_at
            FROM Users
            WHERE email = @email
            LIMIT 1
            """;

        await using var cmd = new MySqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@email", email);

        await using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
            return MapUser(reader);
        return null;
    }

    public async Task<int> CreateAsync(User user)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        const string sql = """
            INSERT INTO Users (username, password, email, phone)
            VALUES (@username, @password, @email, @phone);
            SELECT LAST_INSERT_ID();
            """;

        await using var cmd = new MySqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@username", user.Username);
        cmd.Parameters.AddWithValue("@password", user.Password);
        cmd.Parameters.AddWithValue("@email", user.Email ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@phone", user.Phone ?? (object)DBNull.Value);

        var result = await cmd.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    private static User MapUser(MySqlDataReader reader)
    {
        return new User
        {
            IdUsers = reader.GetInt32("id_users"),
            Username = reader.GetString("username"),
            Password = reader.GetString("password"),
            Email = reader.IsDBNull(reader.GetOrdinal("email")) ? null : reader.GetString("email"),
            Phone = reader.IsDBNull(reader.GetOrdinal("phone")) ? null : reader.GetString("phone"),
            CreatedAt = reader.GetDateTime("created_at"),
            UpdatedAt = reader.GetDateTime("updated_at")
        };
    }
}
