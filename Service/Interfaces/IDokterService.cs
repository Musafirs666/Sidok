using Entity;

namespace Service.Interfaces
{
    public interface IDokterService
    {
        Task<IEnumerable<DokterModel>> GetAllAsync();
        Task AddAsync(DokterModel dokter);
        Task<DokterModel> GetByIdAsync(int id);
        Task UpdateAsync(DokterModel dokter);
        Task DeleteAsync(int id);

        Task<IEnumerable<SpesialisasiModel>> GetSpesialisasiByDokterIdAsync(int dokterId);
        Task AddJadwalAsync(BertugasDiModel bertugasDiModel);
        Task<List<BertugasDiModel>> GetJadwalListAsync(int dokterId);
    }
}
