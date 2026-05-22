using System.Data;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace srv.slots.infrastructure.Persistence;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}

public class MySqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public MySqlConnectionFactory(IConfiguration config)
    {
        var raw = config.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection is not configured.");

        // Ensure SSL is enabled for managed cloud MySQL (Aiven requires it).
        // If the connection string already specifies SslMode, respect it.
        if (!raw.Contains("SslMode", StringComparison.OrdinalIgnoreCase))
        {
            raw = raw.TrimEnd(';') + ";SslMode=Required;";
        }

        _connectionString = raw;
    }

    public IDbConnection CreateConnection() => new MySqlConnection(_connectionString);
}
