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
    /*public void BlockUser(int id, string reason)
    {
        using var connection = new NpgsqlConnection(connectionString);
        var sql = "UPDATE users SET is_deleted = @isDeleted WHERE id = @id";
        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);
        OpenConnection(connection);
        command.ExecuteNonQuery();
        CloseConnection(connection);
        UpdateModificationDate(id);
    }*/
}