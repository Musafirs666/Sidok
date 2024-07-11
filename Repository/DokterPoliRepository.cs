using Entity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using Entity;
using Repository.Interface;


namespace Repository
{
    public class DokterPoliRepository : IDokterPoliRepository
    {
        private readonly string _connectionString;

        public DokterPoliRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public async Task<IEnumerable<DokterModel>> GetDokterByPoliAsync(int poliId)
        {
            using IDbConnection conn = new SqlConnection(_connectionString);
            var dokterDictionary = new Dictionary<int, DokterModel>();

            var sql = @"
        SELECT d.*, s.nama AS SpesialisasiNama
        FROM dokter d
        INNER JOIN bertugas_di bd ON d.Id = bd.dokter_id
        INNER JOIN poli p ON bd.poli_id = p.Id
        LEFT JOIN dokter_spesialisasi ds ON d.Id = ds.dokter_id
        LEFT JOIN spesialisasi s ON ds.spesialisasi_id = s.id
        WHERE bd.poli_id = @PoliId";

            var result = await conn.QueryAsync<DokterModel, string, DokterModel>(sql, 
                (dokter, spesialisasiNama) =>
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
            }, new { PoliId = poliId }, splitOn: "SpesialisasiNama");

            return dokterDictionary.Values;
        }

    }
}
