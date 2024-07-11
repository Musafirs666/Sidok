using System.Collections.Generic;
using System.Threading.Tasks;
using Entity;
using Microsoft.AspNetCore.Components.Routing;

namespace Repository.Interface
{
    public interface IDokterRepository
    {
        Task<IEnumerable<DokterModel>> GetAllAsync();
        Task<DokterModel> GetByIdAsync(int id);
        Task AddAsync(DokterModel dokter);
        Task DeleteAsync(int id);
        Task UpdateAsync(DokterModel dokter);
        Task<IEnumerable<SpesialisasiModel>> GetSpesialisasiByDokterIdAsync(int dokterId);
        Task AddJadwalAsync(BertugasDiModel model);
        Task<IEnumerable<BertugasDiModel>> GetJadwalListAsync(int dokterId);
    }
}
