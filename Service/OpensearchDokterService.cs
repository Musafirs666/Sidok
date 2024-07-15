using Entity;
using Repository.Interface;
using Service.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service
{
    public class OpensearchDokterService : IOpensearchDokterService
    {
        private readonly IOpensearchDokterRepository _opensearchDokterRepository;
        private readonly IDokterRepository _dokterRepository;

        public OpensearchDokterService(IOpensearchDokterRepository opensearchDokterRepository, IDokterRepository dokterRepository)
        {
            _opensearchDokterRepository = opensearchDokterRepository;
            _dokterRepository = dokterRepository;
        }

        public async Task<List<DokterOpensearchModel>> SearchDokterAsync(string searchTerm)
        {
            // Cari data di OpenSearch
            var searchResults = await _opensearchDokterRepository.SearchDokterAsync(searchTerm);

            // Jika ditemukan di OpenSearch, langsung kembalikan hasilnya
            if (searchResults != null && searchResults.Any())
            {
                return searchResults;
            }

            // Lakukan pencarian di SQL Server
            var dokterDetails = await _dokterRepository.SearchDokterByNameAsync(searchTerm);

            // Jika ditemukan di SQL Server, tambahkan atau perbarui di OpenSearch
            if (dokterDetails != null && dokterDetails.Any())
            {
                // Tambahkan atau perbarui data di OpenSearch
                await _opensearchDokterRepository.IndexOrUpdateDokterAsync(dokterDetails);

                // Kembalikan hasil pencarian dari SQL Server setelah diindeks di OpenSearch
                return dokterDetails;
            }

            // Jika tidak ada data ditemukan baik di OpenSearch maupun di SQL Server, Anda dapat memilih cara untuk menangani kasus ini.
            // Di sini, Anda dapat mengembalikan null atau daftar kosong tergantung pada kebutuhan aplikasi Anda.
            return new List<DokterOpensearchModel>(); // atau return null; sesuai kebutuhan
        }

    }
}
