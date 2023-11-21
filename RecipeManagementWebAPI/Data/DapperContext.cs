using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;

using System.Data;

namespace RecipeManagementWebAPI.Data
{
    public class DapperContext
    {
        private readonly string _connectionString;
        internal object getDbConnection;

        public DapperContext(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IDbConnection GetDbConnection()
        {
            return new SqlConnection(_connectionString);
        }
        
    }
}
