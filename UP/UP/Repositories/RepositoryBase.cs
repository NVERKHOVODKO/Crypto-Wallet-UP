using System.Data;
using System.Xml.Serialization;
using Npgsql;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using UP.Models;
using System.Net;

namespace UP.Repositories;

public class RepositoryBase
{
    protected const String connectionString = "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=root;";

    protected void OpenConnection(NpgsqlConnection connection)
    {
        connection.Open();
    }
        
    protected void CloseConnection(NpgsqlConnection connection)
    {
        connection.Close();
    }
}