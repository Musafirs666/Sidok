using Microsoft.Extensions.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using Entity;
using Dapper;
using Repository.Interface;

namespace Repository
{
    public class SpesialisasiRepository : ISpesialisasiRepository
    {
        private readonly IConfiguration _config;

        public SpesialisasiRepository(IConfiguration config)
        {
            _config = config;
        }

        private IDbConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }
        public async Task<IEnumerable<SpesialisasiModel>> GetAllAsync()
        {
            var sql = "SELECT Id, Nama FROM spesialisasi";
            using (var dbConnection = Connection)
            {
                dbConnection.Open();
                return await dbConnection.QueryAsync<SpesialisasiModel>(sql);
            }
        }

    }
}
