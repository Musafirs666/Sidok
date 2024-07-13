using Entity;
using Microsoft.Extensions.Configuration;
using Repository.Interface;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Repository
{
    public class PoliRepository : IPoliRepository
    {
        private readonly string _connectionString;

        public PoliRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<PoliModel>> GetAllAsync()
        {
            using IDbConnection conn = new SqlConnection(_connectionString);

            var sql = @"SELECT * FROM poli";

            // Menggunakan Dapper untuk eksekusi query
            var result = await conn.QueryAsync<PoliModel>(sql);

            return result;
        }

        public async Task<PoliModel> GetByIdAsync(int id)
        {
            using IDbConnection conn = new SqlConnection(_connectionString);

            var sql = @"SELECT * FROM poli WHERE id = @id";

            // Menggunakan Dapper untuk eksekusi query
            var result = await conn.QuerySingleOrDefaultAsync<PoliModel>(sql, new { id });

            return result;
        }

        public async Task<bool> CreateAsync(PoliModel poli)
        {
            using IDbConnection conn = new SqlConnection(_connectionString);

            var sql = @"INSERT INTO poli (nama_poli, lokasi) VALUES (@nama_poli, @lokasi)";

            // Menggunakan Dapper untuk eksekusi query insert
            var affectedRows = await conn.ExecuteAsync(sql, new { poli.nama_poli, poli.lokasi });

            return affectedRows > 0;
        }

        public async Task<bool> UpdateAsync(PoliModel poli)
        {
            using IDbConnection conn = new SqlConnection(_connectionString);

            var sql = @"UPDATE poli SET nama_poli = @nama_poli, lokasi = @lokasi WHERE id = @id";

            // Menggunakan Dapper untuk eksekusi query update
            var affectedRows = await conn.ExecuteAsync(sql, new { poli.nama_poli, poli.lokasi, poli.id });

            return affectedRows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using IDbConnection conn = new SqlConnection(_connectionString);

            var sqlDeleteBertugasDi = @"DELETE FROM bertugas_di WHERE poli_id = @id";
            var sqlDeletePoli = @"DELETE FROM poli WHERE id = @id";

            conn.Open();
            using var transaction = conn.BeginTransaction();

            try
            {
                // Menghapus referensi di tabel bertugas_di
                await conn.ExecuteAsync(sqlDeleteBertugasDi, new { id }, transaction);

                // Menghapus data di tabel poli
                var affectedRows = await conn.ExecuteAsync(sqlDeletePoli, new { id }, transaction);

                transaction.Commit();
                return affectedRows > 0;
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
            finally
            {
                conn.Close();
            }
        }
    

}
}
