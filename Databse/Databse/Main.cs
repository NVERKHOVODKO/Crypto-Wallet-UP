using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace Databse
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=root;";
            using var connection = new NpgsqlConnection(connectionString);
            string dateString = "2023-03-13 12:30:00";
            string format = "yyyy-MM-dd HH:mm:ss";
            DateTime dateTime;
            
            if (DateTime.TryParseExact(dateString, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
            {
                Console.WriteLine(dateTime.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            else
            {
                Console.WriteLine("Unable to parse '{0}'", dateString);
            }
            
            User user = new User(1, "test", "testpassword", "test@test.com", "", "", false, false, 1);
            
            var sql = "INSERT INTO users (login, password, email, creation_date, modification_date, is_deleted, is_blocked, role_id) " +
                      "VALUES (@login, @password, @email, @creation_date, @modification_date, @is_deleted, @is_blocked, @role_id)";

            using var command = new NpgsqlCommand(sql, connection);

            command.Parameters.AddWithValue("@login", user.Login);
            command.Parameters.AddWithValue("@password", user.Password);
            command.Parameters.AddWithValue("@email", user.Email);
            command.Parameters.AddWithValue("@creation_date", dateTime);
            command.Parameters.AddWithValue("@modification_date", dateTime);
            command.Parameters.AddWithValue("@is_deleted", user.IsDeleted);
            command.Parameters.AddWithValue("@is_blocked", user.IsBlocked);
            command.Parameters.AddWithValue("@role_id", user.RoleId);

            connection.Open();

            command.ExecuteNonQuery();

            connection.Close();
            
            
            
            /*var connectionString = "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=root;";
            using var connection = new NpgsqlConnection(connectionString);
            
            var sql = "INSERT INTO coins (id, name, shortname, price_usd, iconpath, dailyimpact, dailyvolume) " +
                      "VALUES (@id, @name, @shortname, @price_usd, @iconpath, @dailyimpact, @dailyvolume)";

            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", 2);

            command.Parameters.AddWithValue("@name", "Etherium");
            command.Parameters.AddWithValue("@shortname", "ETH");
            command.Parameters.AddWithValue("@price_usd", 1700);
            command.Parameters.AddWithValue("@iconpath", "C:");
            command.Parameters.AddWithValue("@dailyimpact", 50);
            command.Parameters.AddWithValue("@dailyvolume", 334);

            
            connection.Open();

            command.ExecuteNonQuery();

            connection.Close();*/
        }
    }
}