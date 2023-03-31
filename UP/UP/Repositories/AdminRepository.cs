using System.Data;
using System.Xml.Serialization;
using Npgsql;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using UP.Models;
using System.Net;

namespace UP.Repositories;

public class AdminRepository: RepositoryBase
{
    private const String connectionString = "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=root;";
    
    public void BlockUser(int id, string reason)
    {
        using var connection = new NpgsqlConnection(connectionString);
        var sql = "UPDATE users SET is_blocked = @isBlocked WHERE id = @id";
        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@isBlocked", true);
        OpenConnection(connection);
        command.ExecuteNonQuery();
        CloseConnection(connection);
        var ur = new Repositories.UserRepository();
        ur.UpdateModificationDate(id);
        CreteBlockingRecordToDatabase(id, reason);
    }
    
    public void CreteBlockingRecordToDatabase(int id, string reason)
    {
        using var connection = new NpgsqlConnection(connectionString);
        var sql = "INSERT INTO blocking (cause, user_id) VALUES (@cause, @user_id)";
        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@cause", reason);
        command.Parameters.AddWithValue("@user_id", id);
        OpenConnection(connection);
        command.ExecuteNonQuery();
        CloseConnection(connection);
    }
    
    
    public void DeleteUser(int id)
    {
        using var connection = new NpgsqlConnection(connectionString);
        var sql = "UPDATE users SET is_deleted = @isDeleted WHERE id = @id";
        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@isDeleted", true);
        OpenConnection(connection);
        command.ExecuteNonQuery();
        CloseConnection(connection);
        var ur = new Repositories.UserRepository();
        ur.UpdateModificationDate(id);
    }
    
    public List<Models.User> GetUserList()
    {
        NpgsqlConnection connection = new NpgsqlConnection(connectionString);
        Models.User user;
        connection.Open();
        var sql = "SELECT * FROM users";
        List<Models.User> users = new List<User>();
        using (var command = new NpgsqlCommand(sql, connection))
        {
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    String login = reader.GetString(1);
                    String password = reader.GetString(2);
                    String email = reader.GetString(3);
                    DateTime creationDate = reader.GetDateTime(4);
                    DateTime modificationDate = reader.GetDateTime(5);
                    Boolean isDeleted = reader.GetBoolean(6);
                    Boolean isBlocked = reader.GetBoolean(7);
                    int roleId = reader.GetInt32(8);
                    users.Add(new Models.User(id, login, password, email, creationDate, 
                        modificationDate, isDeleted, isBlocked, roleId));
                }
            }
        }
        connection.Close();
        return users;
    }

}