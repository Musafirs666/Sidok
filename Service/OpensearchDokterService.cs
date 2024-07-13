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

            // Jika tidak ditemukan di OpenSearch, cari di SQL Server
            if (searchResults == null || !searchResults.Any())
            {
                // Lakukan pencarian di SQL Server
                var dokterDetails = await _dokterRepository.SearchDokterByNameAsync(searchTerm);

                // Jika ditemukan di SQL Server, tambahkan atau perbarui di OpenSearch
                if (dokterDetails != null && dokterDetails.Any())
                {
                    // Tambahkan atau perbarui data di OpenSearch
                    await _opensearchDokterRepository.IndexOrUpdateDokterAsync(dokterDetails);

                    // Kembalikan hasil pencarian dari SQL Server
                    return dokterDetails;
                }
            }

            // Kembalikan hasil pencarian dari OpenSearch atau SQL Server
            return searchResults;
        }
    }
}
