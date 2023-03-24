using System.Xml.Serialization;
using Npgsql;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using UP.Models;
using System.Net;

namespace UP.Database
{
    public class Database
    {
        private String connectionString = "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=root;";


        
        private void OpenConnection(NpgsqlConnection connection)
        {
            connection.Open();
        }
        
        private void CloseConnection(NpgsqlConnection connection)
        {
            connection.Close();
        }
        
        public bool IsLoginUnique(String login)
        {
            using var connection = new NpgsqlConnection(connectionString);
            var sql = "SELECT COUNT(*) FROM users WHERE login = @login";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@login", login);
            OpenConnection(connection);
            int count = Convert.ToInt32(command.ExecuteScalar());
            CloseConnection(connection);
            return Convert.ToBoolean(count);
        }
    }
}