using System.Data;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

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
        _connectionString = config.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection is not configured.");
    }

    public IDbConnection CreateConnection() => new MySqlConnection(_connectionString);
}
