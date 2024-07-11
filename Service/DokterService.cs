using Entity;
using Repository.Interface;
using Service.Interfaces;

namespace Service
{
    public class DokterService : IDokterService
    {
        private readonly IDokterRepository _dokterRepository;

        public DokterService(IDokterRepository dokterRepository)
        {
            _dokterRepository = dokterRepository;
        }

        public async Task<IEnumerable<DokterModel>> GetAllAsync()
        {
            return await _dokterRepository.GetAllAsync();
        }

        public async Task AddAsync(DokterModel dokter)
        {
            await _dokterRepository.AddAsync(dokter);
        }
        
        public async Task AddJadwalAsync(BertugasDiModel bertugasDiModel)
        {
            await _dokterRepository.AddJadwalAsync(bertugasDiModel);
        }

        public async Task<List<BertugasDiModel>> GetJadwalListAsync(int dokterId)
        {
            return (await _dokterRepository.GetJadwalListAsync(dokterId)).ToList();
        }
        public async Task<DokterModel> GetByIdAsync(int id)
        {
            return await _dokterRepository.GetByIdAsync(id);
        }

        public async Task UpdateAsync(DokterModel dokter)
        {
            await _dokterRepository.UpdateAsync(dokter);
        }

        public async Task DeleteAsync(int id)
        {
            await _dokterRepository.DeleteAsync(id);
        }
        public async Task<IEnumerable<SpesialisasiModel>> GetSpesialisasiByDokterIdAsync(int dokterId)
        {
            return await _dokterRepository.GetSpesialisasiByDokterIdAsync(dokterId);
        }

    }
}
