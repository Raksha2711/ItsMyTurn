
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System.Data;

namespace srv.slots.infrastructure.Persistence
{
    public class DapperContext
    {

        private readonly IConfiguration _configuration;

        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IDbConnection CreateConnection()
        {
            return new MySqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));
        }
    }
}
