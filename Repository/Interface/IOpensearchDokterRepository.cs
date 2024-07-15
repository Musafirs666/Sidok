using Entity;

namespace Repository.Interface
{
    public interface IOpensearchDokterRepository
    {
        Task<List<DokterOpensearchModel>> SearchDokterAsync(string searchTerm);
        Task IndexOrUpdateDokterAsync(List<DokterOpensearchModel> dokterDetails);
        Task DeleteDokterAsync(int id);
    }
}
