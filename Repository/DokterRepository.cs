using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using Entity;
using Repository.Interface;

namespace Repository
{
    public class DokterRepository : IDokterRepository
    {
        private readonly string _connectionString;

        public DokterRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<DokterModel>> GetAllAsync()
        {
            using IDbConnection conn = new SqlConnection(_connectionString);
            var dokterDictionary = new Dictionary<int, DokterModel>();

            var sql = @"
                SELECT d.*, s.nama AS SpesialisasiNama
                FROM dokter d
                LEFT JOIN dokter_spesialisasi ds ON d.Id = ds.dokter_id
                LEFT JOIN spesialisasi s ON ds.spesialisasi_id = s.id";

            var result = await conn.QueryAsync<DokterModel, string, DokterModel>(sql, (dokter, spesialisasiNama) =>
            {
                if (!dokterDictionary.TryGetValue(dokter.Id, out var currentDokter))
                {
                    currentDokter = dokter;
                    currentDokter.SpesialisasiData = new List<string>();
                    dokterDictionary.Add(currentDokter.Id, currentDokter);
                }
                if (!string.IsNullOrEmpty(spesialisasiNama))
                {
                    currentDokter.SpesialisasiData.Add(spesialisasiNama);
                }
                return currentDokter;
            }, splitOn: "SpesialisasiNama");

            return dokterDictionary.Values;
        }

        public async Task<DokterModel> GetByIdAsync(int id)
        {
            using IDbConnection conn = new SqlConnection(_connectionString);
            var dokterDictionary = new Dictionary<int, DokterModel>();

            var sql = @"
                SELECT d.*, s.nama AS SpesialisasiNama
                FROM dokter d
                LEFT JOIN dokter_spesialisasi ds ON d.Id = ds.dokter_id
                LEFT JOIN spesialisasi s ON ds.spesialisasi_id = s.id
                WHERE d.Id = @Id";

            var result = await conn.QueryAsync<DokterModel, string, DokterModel>(sql, (dokter, spesialisasiNama) =>
            {
                if (!dokterDictionary.TryGetValue(dokter.Id, out var currentDokter))
                {
                    currentDokter = dokter;
                    currentDokter.SpesialisasiData = new List<string>();
                    dokterDictionary.Add(currentDokter.Id, currentDokter);
                }
                if (!string.IsNullOrEmpty(spesialisasiNama))
                {
                    currentDokter.SpesialisasiData.Add(spesialisasiNama);
                }
                return currentDokter;
            }, new { Id = id }, splitOn: "SpesialisasiNama");

            return dokterDictionary.Values.FirstOrDefault();
        }

        public async Task<IEnumerable<SpesialisasiModel>> GetSpesialisasiByDokterIdAsync(int dokterId)
        {
            using IDbConnection conn = new SqlConnection(_connectionString);
            var query = @"
                SELECT s.*
                FROM spesialisasi s
                INNER JOIN dokter_spesialisasi ds ON s.Id = ds.spesialisasi_id
                WHERE ds.dokter_id = @DokterId";
            return await conn.QueryAsync<SpesialisasiModel>(query, new { DokterId = dokterId });
        }

        public async Task AddAsync(DokterModel dokter)
        {
            using IDbConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            // Generate NIP unik berdasarkan aturan
            dokter.Nip = await GenerateUniqueNipAsync(dokter, conn);

            var sqlDokter = @"
                INSERT INTO dokter (nip, nik, nama, tanggal_lahir, jenis_kelamin, tempat_lahir)
                VALUES (@Nip, @Nik, @Nama, @tanggal_lahir, @jenis_kelamin, @tempat_lahir);
                SELECT CAST(SCOPE_IDENTITY() AS INT)";
            var idDokter = await conn.ExecuteScalarAsync<int>(sqlDokter, dokter);

            foreach (var spesialisasiId in dokter.Spesialisasi)
            {
                var sqlDokterSpesialisasi = @"
                    INSERT INTO dokter_spesialisasi (dokter_id, spesialisasi_id)
                    VALUES (@IdDokter, @IdSpesialisasi)";
                await conn.ExecuteAsync(sqlDokterSpesialisasi, new { IdDokter = idDokter, IdSpesialisasi = spesialisasiId });
            }
        }

        public async Task UpdateAsync(DokterModel dokter)
        {
            using IDbConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            var sqlDokter = @"
                UPDATE dokter
                SET nama = @Nama, tanggal_lahir = @tanggal_lahir,
                    jenis_kelamin = @jenis_kelamin, tempat_lahir = @tempat_lahir
                WHERE Id = @Id";
            await conn.ExecuteAsync(sqlDokter, dokter);

            var sqlDeleteSpesialisasi = @"
                DELETE FROM dokter_spesialisasi WHERE dokter_id = @Id";
            await conn.ExecuteAsync(sqlDeleteSpesialisasi, new { Id = dokter.Id });

            if (dokter.Spesialisasi != null)
            {
                foreach (var spesialisasiId in dokter.Spesialisasi)
                {
                    var sqlInsertSpesialisasi = @"
                        INSERT INTO dokter_spesialisasi (dokter_id, spesialisasi_id)
                        VALUES (@Id, @SpesialisasiId)";
                    await conn.ExecuteAsync(sqlInsertSpesialisasi, new { Id = dokter.Id, SpesialisasiId = spesialisasiId });
                }
            }
        }

        public async Task DeleteAsync(int id)
        {
            using IDbConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            var sqlDeleteSpesialisasi = @"
                DELETE FROM dokter_spesialisasi WHERE dokter_id = @Id";
            await conn.ExecuteAsync(sqlDeleteSpesialisasi, new { Id = id });

            var sqlDeleteDokter = @"
                DELETE FROM dokter WHERE Id = @Id";
            await conn.ExecuteAsync(sqlDeleteDokter, new { Id = id });
        }

        private async Task<bool> NipExistsAsync(string nip, IDbConnection conn)
        {
            var sql = "SELECT COUNT(1) FROM dokter WHERE nip = @Nip";
            return await conn.ExecuteScalarAsync<bool>(sql, new { Nip = nip });
        }

        private async Task<string> GenerateUniqueNipAsync(DokterModel dokter, IDbConnection conn)
        {
            string nip;
            bool exists;

            do
            {
                var yearPart = (DateTime.Now.Year + 5).ToString();
                var birthDatePart = dokter.tanggal_lahir.ToString("ddMMyy");
                var genderPart = dokter.jenis_kelamin == "L" ? "1" : "2";
                var randomPart = new string(Enumerable.Range(0, 2).Select(_ => (char)('A' + new Random().Next(0, 26))).ToArray());

                nip = yearPart + birthDatePart + genderPart + randomPart;
                exists = await NipExistsAsync(nip, conn);
            }
            while (exists);

            return nip;
        }

        public async Task AddJadwalAsync(BertugasDiModel model)
        {
            using IDbConnection conn = new SqlConnection(_connectionString);
            var sql = @"
                INSERT INTO bertugas_di (dokter_id, poli_id, hari)
                VALUES (@DokterId, @PoliId, @Hari)";
            await conn.ExecuteAsync(sql, model);
        }

        public async Task<IEnumerable<BertugasDiModel>> GetJadwalListAsync(int dokterId)
        {
            using IDbConnection conn = new SqlConnection(_connectionString);
            var sql = @"
        SELECT bd.dokter_id AS DokterId, bd.poli_id AS PoliId, bd.hari AS Hari, d.nama AS NamaDokter
        FROM bertugas_di bd
        INNER JOIN dokter d ON bd.dokter_id = d.Id
        WHERE bd.dokter_id = @DokterId";

            var result = await conn.QueryAsync<BertugasDiModel>(sql, new { DokterId = dokterId });
            return result;
        }


    }
}
