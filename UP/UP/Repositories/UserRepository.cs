using System.Net;
using System.Net.Sockets;
using Npgsql;
using UP.Models;
using UP.Models.Base;

namespace UP.Repositories;

public class UserRepository: RepositoryBase
{
    public User? GetUserById(int userId)
    {
        NpgsqlConnection connection = new NpgsqlConnection(connectionString);
        User user;
        connection.Open();
        var sql = "SELECT * FROM users WHERE id = @userId";
        using (var command = new NpgsqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("userId", userId);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string login = reader.GetString(1);
                    string password = reader.GetString(2);
                    string email = reader.GetString(3);
                    DateTime creationDate = reader.GetDateTime(4);
                    DateTime modificationDate = reader.GetDateTime(5);
                    Boolean isDeleted = reader.GetBoolean(6);
                    int roleId = reader.GetInt32(7);
                    Boolean isBlocked = reader.GetBoolean(8);
                    string salt = reader.GetString(9);
                    user = new User(id, login, password, email, creationDate, 
                        modificationDate, isDeleted, isBlocked, roleId, salt);
                    connection.Close();
                    return user;
                }
            }
        }
        connection.Close();
        return null;
    }
    
    
    public List<PreviosPasswords> GetUserPasswordsHistory(int userId)
    {
        NpgsqlConnection connection = new NpgsqlConnection(connectionString);
        List<PreviosPasswords> passwords = new List<PreviosPasswords>();
        connection.Open();
        var sql = "SELECT * FROM previos_passwords WHERE user_id = @user_id";
        using (var command = new NpgsqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("user_id", userId);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    int userIdRead = reader.GetInt32(1);
                    string password = reader.GetString(2);
                    passwords.Add(new PreviosPasswords(id, userIdRead, password));
                }
            }
        }
        connection.Close();
        return passwords;
    }
    
    public List<LoginHistory> GetUserLoginHistory(int userId)
    {
        NpgsqlConnection connection = new NpgsqlConnection(connectionString);
        List<LoginHistory> loginHistory = new List<LoginHistory>();
        connection.Open();
        var sql = "SELECT * FROM login_history WHERE user_id = @user_id ORDER BY id DESC";
        using (var command = new NpgsqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("user_id", userId);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string ip = reader.GetString(1);
                    DateTime date = reader.GetDateTime(2);
                    int userIdRead = reader.GetInt32(3);
                    loginHistory.Add(new LoginHistory(id, ip, date, userIdRead));
                }
            }
        }
        connection.Close();
        return loginHistory;
    }
    
    public void WriteNewUserToDatabase(string login, string password)
    {
        var ar = new AuthorizationRepository();
        string salt = ar.GetSalt();
        password = Convert.ToString(ar.Hash(password));
        password = Convert.ToString(ar.Hash(password + salt));
        DateTime curDateTime = DateTime.Now;
        DateTime modificationDateTime = DateTime.Now;
        var user = new User(1, login, password, "emailAdress", curDateTime, 
            modificationDateTime, false, false, 1, salt);
        using var connection = new NpgsqlConnection(connectionString);
        var sql = "INSERT INTO users (login, password, email, creation_date, modification_date, is_deleted, is_blocked, role_id, salt) " +
                  "VALUES (@login, @password, @email, @creation_date, @modification_date, @is_deleted, @is_blocked, @role_id, @salt)";
        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@login", user.Login);
        command.Parameters.AddWithValue("@password", user.Password);
        command.Parameters.AddWithValue("@email", user.Email);
        command.Parameters.AddWithValue("@creation_date", user.CreationData);
        command.Parameters.AddWithValue("@modification_date", user.ModificationDate);
        command.Parameters.AddWithValue("@is_deleted", user.IsDeleted);
        command.Parameters.AddWithValue("@is_blocked", user.IsBlocked);
        command.Parameters.AddWithValue("@role_id", user.RoleId);
        command.Parameters.AddWithValue("@salt", user.Salt);
        OpenConnection(connection);
        command.ExecuteNonQuery();
        CloseConnection(connection);
    }
        
    public void SaveAccountLoginHistory(int id)
    {
        try
        {
            string? ipAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList
                .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork)?.ToString();
            var sql = "INSERT INTO login_history (ip, date, user_id) " +
                      "VALUES (@ip, @date, @user_id)";
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            using var command = new NpgsqlCommand(sql, connection);
            if (ipAddress != null)
            {
                command.Parameters.AddWithValue("@ip", ipAddress);
                command.Parameters.AddWithValue("@date", DateTime.Now);
                command.Parameters.AddWithValue("@user_id", id);
                OpenConnection(connection);
                command.ExecuteNonQuery();
                CloseConnection(connection);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error. Can't update login history");
        }
    }

    public List<Coin> GetUserCoins(int userId)
    {
        using (var connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();
            string query = "SELECT c.id,c.quantity, c.shortname " +
                           "FROM coins c " +
                           "INNER JOIN l_users_coins l ON l.coin_id = c.id " +
                           "WHERE l.user_id = @userId";
            List<Coin> coins = new List<Coin>();
            using (var cmd = new NpgsqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@userId", userId);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int coinId = reader.GetInt32(0);
                        double coinQuantity = reader.GetDouble(1);
                        string shortname = reader.GetString(2);
                        Coin coin = new Coin(coinId, coinQuantity, shortname);
                        coins.Add(coin);
                    }
                }
            }
            connection.Close();
            return coins;
        }
    }
    
    public async Task<List<CoinsInformation>> GetUserCoinsFull(int userId)
    {
        var coins = GetUserCoins(userId);
        List<CoinsInformation>  coinsFull = new List<CoinsInformation>();
        int i = 0;
        var cr = new CurrencyRepository();
        foreach (var coin in coins)
        {
            CoinsInformation temp = await cr.GetFullCoinInformation(coin.ShortName);
            coinsFull.Add(new (i, temp.ShortName, temp.FullName, temp.DailyVolume, temp.DailyImpact, temp.Price, temp.PercentagePriceChangePerDay, coins[i].Quantity));
            i++;
        }

        return coinsFull;
    }

    public double GetCoinQuantityInUserWallet(int userId, string coinShortname)
    {
        try
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT c.id,c.quantity, c.shortname " +
                               "FROM coins c " +
                               "INNER JOIN l_users_coins l ON l.coin_id = c.id " +
                               "WHERE l.user_id = @userId AND c.shortname = @coinShortname";
                List<Coin> coins = new List<Coin>();
                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@coinShortname", coinShortname);
                    using (var reader = cmd.ExecuteReader( ))
                    {
                        while (reader.Read())
                        {
                            int coinId = reader.GetInt32(0);
                            double coinQuantity = reader.GetDouble(1);
                            string shortname = reader.GetString(2);
                            Coin coin = new Coin(coinId, coinQuantity, shortname);
                            coins.Add(coin);
                        }
                    }
                }
                connection.Close();
                return coins[0].Quantity;
            }

            /*string sql = "SELECT c.quantity " +
                         "FROM coins c " +
                         "INNER JOIN l_users_coins l ON l.coin_id = c.id " +
                         "WHERE l.user_id = @userId AND c.shortname = @coinShortname";
            double quantity = 0;
            var connection = new NpgsqlConnection(connectionString);
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@user_id", userId);
            command.Parameters.AddWithValue("@shortname", coinShortname);
            OpenConnection(connection);
            quantity = Convert.ToDouble(command.ExecuteScalar());
            CloseConnection(connection);
            return quantity;*/
        }
        catch (Exception)
        {
            return 0.0;
        }
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
    
    public string? GetUserBlockingReason(int userId)
    {
        using var connection = new NpgsqlConnection(connectionString);
        var sql = "SELECT cause FROM blocking WHERE user_id = @userId";
        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@userId", userId);
        OpenConnection(connection);
        string? reason = command.ExecuteScalar()?.ToString();
        CloseConnection(connection);
        return reason;
    }
        
    public void EditUser(int id,User user)
    {
        using var connection = new NpgsqlConnection(connectionString);
        var sql = "UPDATE users SET (login, password, email, creation_date, modification_date, is_deleted, is_blocked, role_id) " +
                  "= (@login, @password, @email, @creation_date, @modification_date, @is_deleted, @is_blocked, @role_id) WHERE id = @id";
        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@login", user.Login);
        command.Parameters.AddWithValue("@password", user.Password);
        command.Parameters.AddWithValue("@email", user.Email);
        command.Parameters.AddWithValue("@creation_date", user.CreationData);
        command.Parameters.AddWithValue("@modification_date", DateTime.Now);
        command.Parameters.AddWithValue("@is_deleted", user.IsDeleted);
        command.Parameters.AddWithValue("@is_blocked", user.IsBlocked);
        command.Parameters.AddWithValue("@role_id", user.RoleId);
        OpenConnection(connection);
        command.ExecuteNonQuery();
        CloseConnection(connection);
    }
        
    public void SetUserStatusDel(int id, bool isDeleted)
    {
        using var connection = new NpgsqlConnection(connectionString);
        var sql = "UPDATE users SET is_deleted = @isDeleted WHERE id = @id";
        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@isDeleted", isDeleted);
        OpenConnection(connection);
        command.ExecuteNonQuery();
        CloseConnection(connection);
        UpdateModificationDate(id);
    }
        
    public void SetUserStatusBlock(int id, bool isBlocked)
    {
        using var connection = new NpgsqlConnection(connectionString);
        var sql = "UPDATE users SET is_blocked = @isBlocked WHERE id = @id";
        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@isBlocked", isBlocked);
        OpenConnection(connection);
        command.ExecuteNonQuery();
        CloseConnection(connection);
        UpdateModificationDate(id);
    }
        
    public void ChangeUserLogin(int id, string login)
    {
        using var connection = new NpgsqlConnection(connectionString);
        var sql = "UPDATE users SET login = @login WHERE id = @id";
        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@login", login);
        OpenConnection(connection);
        command.ExecuteNonQuery();
        CloseConnection(connection);
        UpdateModificationDate(id);
    }
    
    public void ChangeUserPassword(int id, string password)
    {
        var ar = new Repositories.AuthorizationRepository();
        var ur = new Repositories.UserRepository();
        Models.User user = GetUserById(id);
        password = Convert.ToString(ar.Hash(password));
        password = Convert.ToString(ar.Hash(password + user.Salt));
        using var connection = new NpgsqlConnection(connectionString);
        var sql = "UPDATE users SET password = @password WHERE id = @id";
        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@password", password);
        OpenConnection(connection);
        command.ExecuteNonQuery();
        CloseConnection(connection);
        UpdateModificationDate(id);
    }
    
    public void ChangeUserEmail(int id, string email)
    {
        using var connection = new NpgsqlConnection(connectionString);
        var sql = "UPDATE users SET email = @email WHERE id = @id";
        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@email", email);
        OpenConnection(connection);
        command.ExecuteNonQuery();
        CloseConnection(connection);
        UpdateModificationDate(id);
    }
    
    
    
    public void DeleteUser(int id)
    {
        using var connection = new NpgsqlConnection(connectionString);
        var sql = "DELETE users WHERE id = @id";
        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);
        OpenConnection(connection);
        command.ExecuteNonQuery();
        CloseConnection(connection);
        UpdateModificationDate(id);
    }
        
    public void ChangeUserName(int id, string newLogin)
    {
        using var connection = new NpgsqlConnection(connectionString);
        var sql = "UPDATE users SET login = @newLogin WHERE id = @id";
        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@newLogin", newLogin);
        OpenConnection(connection);
        command.ExecuteNonQuery();
        CloseConnection(connection);
        UpdateModificationDate(id);
    }

    public void AddPasswordInHistoryList(int userId, string password)
    {
        using var connection = new NpgsqlConnection(connectionString);
        var sql = "INSERT INTO previos_passwords  (user_id, password) VALUES (@user_id, @password)";
        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@user_id", userId);
        command.Parameters.AddWithValue("@password", password);
        OpenConnection(connection);
        command.ExecuteNonQuery();
        CloseConnection(connection);
    }
        
    public void ChangePassword(int id, string password)
    {
        using var connection = new NpgsqlConnection(connectionString);
        var sql = "UPDATE users SET password = @password WHERE id = @id";
        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@password", password);
        OpenConnection(connection);
        command.ExecuteNonQuery();
        CloseConnection(connection);
        AddPasswordInHistoryList(id, password);
        UpdateModificationDate(id);
    }
    
    public void UpdateModificationDate(int id)
    {
        using var connection = new NpgsqlConnection(connectionString);
        var sql = "UPDATE users SET modification_date = @modification_date WHERE id = @id";
        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@modification_date", DateTime.Now);
        OpenConnection(connection);
        command.ExecuteNonQuery();
        CloseConnection(connection);
    }

    public User? GetUserIdByLogin(string login)
    {
        NpgsqlConnection connection = new NpgsqlConnection(connectionString);
        User user;
        connection.Open();
        var sql = "SELECT * FROM users WHERE login = @login LIMIT 1";
        using (var command = new NpgsqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("@login", login);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    login = reader.GetString(1);
                    string password = reader.GetString(2);
                    string email = reader.GetString(3);
                    DateTime creationDate = reader.GetDateTime(4);
                    DateTime modificationDate = reader.GetDateTime(5);
                    bool isDeleted = reader.GetBoolean(6);
                    int roleId = reader.GetInt32(7);
                    bool isBlocked = reader.GetBoolean(8);
                    string salt = reader.GetString(9);
                    user = new User(id, login, password, email, creationDate, 
                        modificationDate, isDeleted, isBlocked, roleId, salt);
                    connection.Close();
                    return user;
                }
            }
        }
        connection.Close(); // закрываем подключение
        return null;
    }
    
        
    
    public User? Login(string login, string password)
    {
        NpgsqlConnection connection = new NpgsqlConnection(connectionString);
        connection.Open();
        User? user = GetUserIdByLogin(login);
        var ar = new AuthorizationRepository();
        password = ar.Hash(password);
        password = ar.Hash(password + user.Salt);

        var sql = "SELECT * FROM users WHERE login = @login AND password = @password";
        using (var command = new NpgsqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("login", user.Login);
            command.Parameters.AddWithValue("password", password);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    login = reader.GetString(1);
                    password = reader.GetString(2);
                    string email = reader.GetString(3);
                    DateTime creationDate = reader.GetDateTime(4);
                    DateTime modificationDate = reader.GetDateTime(5);
                    bool isDeleted = reader.GetBoolean(6);
                    int roleId = reader.GetInt32(7);
                    bool isBlocked = reader.GetBoolean(8);
                    string salt = reader.GetString(9);
                    user = new User(id, login, password, email, creationDate, 
                        modificationDate, isDeleted, isBlocked, roleId, salt);
                    connection.Close();
                    return user;
                }
            }
        }
        connection.Close();
        return null;
    }
}