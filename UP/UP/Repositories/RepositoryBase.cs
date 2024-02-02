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
    protected const string connectionString = "Host=localhost;Port=5432;Database=cryptowallet;Username=postgres;Password=postgres;";
    
    protected const string apiKey = "4da2c4791b9c285b22c1bf08bc36f304ab2ca80bc901504742b9a42a814c4614";
    //protected const string apiKey = "9b2de8c26d9088c31d9d561e05040412917c8077ddf256f68fac9f94a7511fa7";
    
    protected void OpenConnection(NpgsqlConnection connection)
    {
        connection.Open();
    }
        
    protected void CloseConnection(NpgsqlConnection connection)
    {
        connection.Close();
    }
}