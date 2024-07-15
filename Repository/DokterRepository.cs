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
        private readonly IOpensearchDokterRepository _opensearchDokterRepository;

        public DokterRepository(IConfiguration configuration, IOpensearchDokterRepository opensearchDokterRepository)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _opensearchDokterRepository = opensearchDokterRepository;
        }

        private IDbConnection CreateConnection()
        {
            var connection = new SqlConnection(_connectionString);
            connection.Open();
            return connection;
        }

        public async Task<List<DokterModel>> GetDoktersByIdsAsync(IEnumerable<int> ids)
        {
            using (var connection = CreateConnection())
            {
                var sql = "SELECT * FROM Dokter WHERE Id IN @Ids";
                var result = await connection.QueryAsync<DokterModel>(sql, new { Ids = ids });
                return result.ToList();
            }
        }

        public async Task<List<DokterOpensearchModel>> SearchDokterByNameAsync(string searchTerm)
        {
            using (var connection = CreateConnection())
            {
                // Kueri untuk mengambil data dokter dan spesialisasi terkait
                var query = @"
            SELECT d.Id, d.Nama, d.Nip, d.Nik, d.Jenis_kelamin, s.Nama AS Spesialisasi
            FROM dokter d
            LEFT JOIN dokter_spesialisasi ds ON d.Id = ds.Dokter_Id
            LEFT JOIN spesialisasi s ON ds.spesialisasi_id = s.Id
            WHERE d.Nama LIKE @searchTerm";

                var parameters = new { searchTerm = $"%{searchTerm}%" };
                var dokterDictionary = new Dictionary<int, DokterOpensearchModel>();

                // Menggunakan QueryAsync dengan multi mapping untuk memetakan data
                var result = await connection.QueryAsync<DokterOpensearchModel, string, DokterOpensearchModel>(
                    query,
                    (dokter, spesialisasi) =>
                    {
                        if (!dokterDictionary.TryGetValue(dokter.Id, out var dokterEntry))
                        {
                            dokterEntry = dokter;
                            dokterEntry.Spesialisasi = new List<string>();
                            dokterDictionary.Add(dokter.Id, dokterEntry);
                        }

                        if (!string.IsNullOrEmpty(spesialisasi))
                        {
                            dokterEntry.Spesialisasi.Add(spesialisasi);
                        }

                        return dokterEntry;
                    },
                    parameters,
                    splitOn: "Spesialisasi");

                return dokterDictionary.Values.ToList();
            }
        }


        public async Task<IEnumerable<DokterModel>> GetAllAsync()
        {
            using (var connection = CreateConnection())
            {
                var dokterDictionary = new Dictionary<int, DokterModel>();

                var sql = @"
                    SELECT d.*, s.nama AS SpesialisasiNama
                    FROM dokter d
                    LEFT JOIN dokter_spesialisasi ds ON d.Id = ds.dokter_id
                    LEFT JOIN spesialisasi s ON ds.spesialisasi_id = s.id";

                var result = await connection.QueryAsync<DokterModel, string, DokterModel>(sql, (dokter, spesialisasiNama) =>
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
        }

        public async Task<DokterModel> GetByIdAsync(int id)
        {
            using (var connection = CreateConnection())
            {
                var dokterDictionary = new Dictionary<int, DokterModel>();

                var sql = @"
                    SELECT d.*, s.nama AS SpesialisasiNama
                    FROM dokter d
                    LEFT JOIN dokter_spesialisasi ds ON d.Id = ds.dokter_id
                    LEFT JOIN spesialisasi s ON ds.spesialisasi_id = s.id
                    WHERE d.Id = @Id";

                var result = await connection.QueryAsync<DokterModel, string, DokterModel>(sql, (dokter, spesialisasiNama) =>
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
        }

        public async Task<IEnumerable<SpesialisasiModel>> GetSpesialisasiByDokterIdAsync(int dokterId)
        {
            using (var connection = CreateConnection())
            {
                var query = @"
                    SELECT s.*
                    FROM spesialisasi s
                    INNER JOIN dokter_spesialisasi ds ON s.Id = ds.spesialisasi_id
                    WHERE ds.dokter_id = @DokterId";
                return await connection.QueryAsync<SpesialisasiModel>(query, new { DokterId = dokterId });
            }
        }

        public async Task AddAsync(DokterModel dokter)
        {
            using (var connection = CreateConnection())
            {
                // Generate NIP unik berdasarkan aturan
                dokter.Nip = await GenerateUniqueNipAsync(dokter);

                var sqlDokter = @"
            INSERT INTO dokter (nip, nik, nama, tanggal_lahir, jenis_kelamin, tempat_lahir)
            VALUES (@Nip, @Nik, @Nama, @Tanggal_Lahir, @Jenis_Kelamin, @Tempat_Lahir);
            SELECT CAST(SCOPE_IDENTITY() AS INT)";
                var idDokter = await connection.ExecuteScalarAsync<int>(sqlDokter, dokter);
                dokter.Id = idDokter;

                foreach (var spesialisasiId in dokter.Spesialisasi)
                {
                    var sqlDokterSpesialisasi = @"
                INSERT INTO dokter_spesialisasi (dokter_id, spesialisasi_id)
                VALUES (@IdDokter, @IdSpesialisasi)";
                    await connection.ExecuteAsync(sqlDokterSpesialisasi, new { IdDokter = idDokter, IdSpesialisasi = spesialisasiId });
                }

                // Ambil data spesialisasi untuk dokter
                var spesialisasiList = await connection.QueryAsync<string>(
                    "SELECT s.nama FROM spesialisasi s JOIN dokter_spesialisasi ds ON s.id = ds.spesialisasi_id WHERE ds.dokter_id = @IdDokter",
                    new { IdDokter = idDokter });

                // Index or update in OpenSearch
                var dokterOpensearchModel = new DokterOpensearchModel
                {
                    Id = dokter.Id,
                    Nama = dokter.Nama,
                    Nip = dokter.Nip,
                    Nik = dokter.Nik,
                    Jenis_Kelamin = dokter.JenisKelaminDeskripsi,
                    Spesialisasi = spesialisasiList.ToList()
                };
                await _opensearchDokterRepository.IndexOrUpdateDokterAsync(new List<DokterOpensearchModel> { dokterOpensearchModel });
            }
        }

        public async Task UpdateAsync(DokterModel dokter)
        {
            using (var connection = CreateConnection())
            {
                // Pastikan NIP tidak berubah
                var existingDokter = await connection.QueryFirstOrDefaultAsync<DokterModel>(
                    "SELECT nip, nik FROM dokter WHERE Id = @Id", new { dokter.Id });
                if (existingDokter == null)
                {
                    throw new Exception("Dokter not found");
                }

                dokter.Nip = existingDokter.Nip;
                dokter.Nik = existingDokter.Nik;

                var sqlDokter = @"
    UPDATE dokter
    SET nama = @Nama, tanggal_lahir = @Tanggal_Lahir,
        jenis_kelamin = @Jenis_Kelamin, tempat_lahir = @Tempat_Lahir
    WHERE Id = @Id";
                await connection.ExecuteAsync(sqlDokter, dokter);

                var sqlDeleteSpesialisasi = @"
    DELETE FROM dokter_spesialisasi WHERE dokter_id = @Id";
                await connection.ExecuteAsync(sqlDeleteSpesialisasi, new { Id = dokter.Id });

                if (dokter.Spesialisasi != null)
                {
                    foreach (var spesialisasiId in dokter.Spesialisasi)
                    {
                        var sqlDokterSpesialisasi = @"
            INSERT INTO dokter_spesialisasi (dokter_id, spesialisasi_id)
            VALUES (@IdDokter, @IdSpesialisasi)";
                        await connection.ExecuteAsync(sqlDokterSpesialisasi, new { IdDokter = dokter.Id, IdSpesialisasi = spesialisasiId });
                    }
                }

                // Ambil data spesialisasi untuk dokter
                var spesialisasiList = await connection.QueryAsync<string>(
                    "SELECT s.nama FROM spesialisasi s JOIN dokter_spesialisasi ds ON s.id = ds.spesialisasi_id WHERE ds.dokter_id = @IdDokter",
                    new { IdDokter = dokter.Id });

                // Index atau update di OpenSearch dengan data terbaru
                var dokterOpensearchModel = new DokterOpensearchModel
                {
                    Id = dokter.Id,
                    Nama = dokter.Nama,
                    Nip = dokter.Nip,
                    Nik = dokter.Nik,
                    Jenis_Kelamin = dokter.JenisKelaminDeskripsi,
                    Spesialisasi = spesialisasiList.ToList()
                };
                await _opensearchDokterRepository.IndexOrUpdateDokterAsync(new List<DokterOpensearchModel> { dokterOpensearchModel });
            }
        }




        public async Task DeleteAsync(int id)
        {
            using (var connection = CreateConnection())
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    // Menghapus referensi dari tabel dokter_spesialisasi
                    var sqlDeleteSpesialisasi = @"DELETE FROM dokter_spesialisasi WHERE dokter_id = @Id";
                    await connection.ExecuteAsync(sqlDeleteSpesialisasi, new { Id = id }, transaction);

                    // Menghapus referensi dari tabel bertugas_di
                    var sqlDeleteBertugasDi = @"DELETE FROM bertugas_di WHERE dokter_id = @Id";
                    await connection.ExecuteAsync(sqlDeleteBertugasDi, new { Id = id }, transaction);

                    // Menghapus entri dari tabel dokter
                    var sqlDeleteDokter = @"DELETE FROM dokter WHERE Id = @Id";
                    await connection.ExecuteAsync(sqlDeleteDokter, new { Id = id }, transaction);

                    await _opensearchDokterRepository.DeleteDokterAsync(id);
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }

        }

        private async Task<bool> NipExistsAsync(string nip)
        {
            using (var connection = CreateConnection())
            {
                var sql = "SELECT COUNT(1) FROM dokter WHERE nip = @Nip";
                return await connection.ExecuteScalarAsync<bool>(sql, new { Nip = nip });
            }
        }

        private async Task<string> GenerateUniqueNipAsync(DokterModel dokter)
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
                exists = await NipExistsAsync(nip);
            }
            while (exists);

            return nip;
        }

        public async Task AddJadwalAsync(BertugasDiModel model)
        {
            using (var connection = CreateConnection())
            {
                var sql = @"
                    INSERT INTO bertugas_di (dokter_id, poli_id, hari)
                    VALUES (@DokterId, @PoliId, @Hari)";
                await connection.ExecuteAsync(sql, model);
            }
        }

        public async Task<IEnumerable<BertugasDiModel>> GetJadwalListAsync(int dokterId)
        {
            using (var connection = CreateConnection())
            {
                var sql = @"
                    SELECT bd.dokter_id AS DokterId, bd.poli_id AS PoliId, bd.hari AS Hari, d.nama AS NamaDokter
                    FROM bertugas_di bd
                    INNER JOIN dokter d ON bd.dokter_id = d.Id
                    WHERE bd.dokter_id = @DokterId";

                var result = await connection.QueryAsync<BertugasDiModel>(sql, new { DokterId = dokterId });
                return result;
            }
        }
    }
}
