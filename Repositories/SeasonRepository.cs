using CoachManagement_Api.Entity;
using CoachManagement_Api.Repositories.interfaces;
using MySqlConnector;

namespace CoachManagement_Api.Repositories;

public class SeasonRepository : ISeasonRepository
{
    private readonly string _connectionString;
    public SeasonRepository(IConfiguration configuration) => _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    public async Task<IReadOnlyList<Season>> GetAllAsync(){await using var c=new MySqlConnection(_connectionString);await c.OpenAsync();await using var cmd=new MySqlCommand("SELECT id_seasons,startDate,endDate,name,notes,created_at,updated_at FROM Seasons ORDER BY startDate DESC",c);var l=new List<Season>();await using var r=await cmd.ExecuteReaderAsync();while(await r.ReadAsync())l.Add(Map(r));return l;}
    public async Task<Season?> GetByIdAsync(int id){await using var c=new MySqlConnection(_connectionString);await c.OpenAsync();await using var cmd=new MySqlCommand("SELECT id_seasons,startDate,endDate,name,notes,created_at,updated_at FROM Seasons WHERE id_seasons=@id LIMIT 1",c);cmd.Parameters.AddWithValue("@id",id);await using var r=await cmd.ExecuteReaderAsync();return await r.ReadAsync()?Map(r):null;}
    public async Task<int> CreateAsync(Season e){await using var c=new MySqlConnection(_connectionString);await c.OpenAsync();await using var cmd=new MySqlCommand("INSERT INTO Seasons (startDate,endDate,name,notes) VALUES (@s,@e,@n,@no); SELECT LAST_INSERT_ID();",c);cmd.Parameters.AddWithValue("@s",e.startDate);cmd.Parameters.AddWithValue("@e",e.endDate);cmd.Parameters.AddWithValue("@n",string.IsNullOrWhiteSpace(e.name)?DBNull.Value:e.name!);cmd.Parameters.AddWithValue("@no",string.IsNullOrWhiteSpace(e.notes)?DBNull.Value:e.notes!);return Convert.ToInt32(await cmd.ExecuteScalarAsync());}
    public async Task<bool> UpdateAsync(Season e){await using var c=new MySqlConnection(_connectionString);await c.OpenAsync();await using var cmd=new MySqlCommand("UPDATE Seasons SET startDate=@s,endDate=@e,name=@n,notes=@no WHERE id_seasons=@id",c);cmd.Parameters.AddWithValue("@id",e.id_seasons);cmd.Parameters.AddWithValue("@s",e.startDate);cmd.Parameters.AddWithValue("@e",e.endDate);cmd.Parameters.AddWithValue("@n",string.IsNullOrWhiteSpace(e.name)?DBNull.Value:e.name!);cmd.Parameters.AddWithValue("@no",string.IsNullOrWhiteSpace(e.notes)?DBNull.Value:e.notes!);return await cmd.ExecuteNonQueryAsync()>0;}
    public async Task<bool> DeleteAsync(int id){await using var c=new MySqlConnection(_connectionString);await c.OpenAsync();await using var cmd=new MySqlCommand("DELETE FROM Seasons WHERE id_seasons=@id",c);cmd.Parameters.AddWithValue("@id",id);return await cmd.ExecuteNonQueryAsync()>0;}
    private static Season Map(MySqlDataReader r)=>new(){id_seasons=r.GetInt32("id_seasons"),startDate=r.GetDateTime("startDate"),endDate=r.GetDateTime("endDate"),name=r.IsDBNull(r.GetOrdinal("name"))?null:r.GetString("name"),notes=r.IsDBNull(r.GetOrdinal("notes"))?null:r.GetString("notes"),created_at=r.GetDateTime("created_at"),updated_at=r.GetDateTime("updated_at")};
}
