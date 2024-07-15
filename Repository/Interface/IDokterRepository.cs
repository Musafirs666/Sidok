using Entity;

namespace Repository.Interface
{
    public interface IDokterRepository
    {
        Task<IEnumerable<DokterModel>> GetAllAsync();
        Task<List<DokterModel>> GetDoktersByIdsAsync(IEnumerable<int> ids);
        Task<DokterModel> GetByIdAsync(int id);
        Task AddAsync(DokterModel dokter);
        Task DeleteAsync(int id);
        Task UpdateAsync(DokterModel dokter);
        Task<IEnumerable<SpesialisasiModel>> GetSpesialisasiByDokterIdAsync(int dokterId);
        Task AddJadwalAsync(BertugasDiModel model);
        Task<IEnumerable<BertugasDiModel>> GetJadwalListAsync(int dokterId);
        Task<List<DokterOpensearchModel>> SearchDokterByNameAsync(string searchTerm);
    }
}
