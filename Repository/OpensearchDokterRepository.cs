using Entity;
using OpenSearch.Client;
using Repository.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repository
{
    public class OpensearchDokterRepository : IOpensearchDokterRepository
    {
        private readonly IOpenSearchClient _client;

        public OpensearchDokterRepository(IOpenSearchClient client)
        {
            _client = client;
        }

        public async Task<List<DokterOpensearchModel>> SearchDokterAsync(string searchTerm)
        {
            var searchResponse = await _client.SearchAsync<DokterOpensearchModel>(s => s
                .Query(q => q
                    .MultiMatch(m => m
                        .Fields(f => f
                            .Field(d => d.Nama)
                            .Field(d => d.Nip)
                            .Field(d => d.Nik)
                            .Field(d => d.Jenis_Kelamin))
                        .Query(searchTerm)
                    )
                )
            );

            if (!searchResponse.IsValid)
            {
                throw new Exception($"Error searching documents: {searchResponse.OriginalException.Message}");
            }

            return searchResponse.Documents.ToList();
        }


        public async Task IndexOrUpdateDokterAsync(List<DokterOpensearchModel> dokterDetails)
        {
            // Lakukan index atau update dokumen di OpenSearch
            var bulkResponse = await _client.BulkAsync(b => b
                .IndexMany(dokterDetails, (descriptor, dokter) => descriptor
                    .Index("dokter_index") 
                    .Id(dokter.Id) // Atur ID
                )
            );

            if (!bulkResponse.IsValid)
            {
                throw new Exception($"Failed to index or update documents: {bulkResponse.OriginalException.Message}");
            }
        }
        public async Task DeleteDokterAsync(int id)
        {
            var deleteResponse = await _client.DeleteAsync<DokterOpensearchModel>(id, d => d
                .Index("dokter_index")
            );

            if (!deleteResponse.IsValid)
            {
                throw new Exception($"Failed to delete document: {deleteResponse.OriginalException.Message}");
            }
        }
    }
}
