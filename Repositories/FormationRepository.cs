using CoachManagement_Api.Entity;
using CoachManagement_Api.Repositories.interfaces;
using MySqlConnector;

namespace CoachManagement_Api.Repositories;

public class FormationRepository : IFormationRepository
{
    private readonly string _connectionString;
    public FormationRepository(IConfiguration configuration) => _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    public async Task<IReadOnlyList<Formation>> GetAllAsync(){await using var c=new MySqlConnection(_connectionString);await c.OpenAsync();await using var cmd=new MySqlCommand("SELECT id_formations,format,description FROM Formations ORDER BY id_formations",c);var l=new List<Formation>();await using var r=await cmd.ExecuteReaderAsync();while(await r.ReadAsync())l.Add(new Formation{id_formations=r.GetInt32("id_formations"),format=r.GetString("format"),description=r.IsDBNull(r.GetOrdinal("description"))?null:r.GetString("description")});return l;}
    public async Task<Formation?> GetByIdAsync(int id){await using var c=new MySqlConnection(_connectionString);await c.OpenAsync();await using var cmd=new MySqlCommand("SELECT id_formations,format,description FROM Formations WHERE id_formations=@id LIMIT 1",c);cmd.Parameters.AddWithValue("@id",id);await using var r=await cmd.ExecuteReaderAsync();if(!await r.ReadAsync())return null;return new Formation{id_formations=r.GetInt32("id_formations"),format=r.GetString("format"),description=r.IsDBNull(r.GetOrdinal("description"))?null:r.GetString("description")};}
    public async Task<int> CreateAsync(Formation e){await using var c=new MySqlConnection(_connectionString);await c.OpenAsync();await using var cmd=new MySqlCommand("INSERT INTO Formations (format,description) VALUES (@f,@d); SELECT LAST_INSERT_ID();",c);cmd.Parameters.AddWithValue("@f",e.format);cmd.Parameters.AddWithValue("@d",string.IsNullOrWhiteSpace(e.description)?DBNull.Value:e.description!);return Convert.ToInt32(await cmd.ExecuteScalarAsync());}
    public async Task<bool> UpdateAsync(Formation e){await using var c=new MySqlConnection(_connectionString);await c.OpenAsync();await using var cmd=new MySqlCommand("UPDATE Formations SET format=@f,description=@d WHERE id_formations=@id",c);cmd.Parameters.AddWithValue("@id",e.id_formations);cmd.Parameters.AddWithValue("@f",e.format);cmd.Parameters.AddWithValue("@d",string.IsNullOrWhiteSpace(e.description)?DBNull.Value:e.description!);return await cmd.ExecuteNonQueryAsync()>0;}
    public async Task<bool> DeleteAsync(int id){await using var c=new MySqlConnection(_connectionString);await c.OpenAsync();await using var cmd=new MySqlCommand("DELETE FROM Formations WHERE id_formations=@id",c);cmd.Parameters.AddWithValue("@id",id);return await cmd.ExecuteNonQueryAsync()>0;}
}
