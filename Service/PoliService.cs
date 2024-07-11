using Entity;
using Repository.Interface;
using Service.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service
{
    public class PoliService : IPoliService
    {
        private readonly IPoliRepository _poliRepository;

        public PoliService(IPoliRepository poliRepository)
        {
            _poliRepository = poliRepository;
        }

        public async Task<IEnumerable<PoliModel>> GetAllAsync()
        {
            return await _poliRepository.GetAllAsync();
        }

        public async Task<PoliModel> GetByIdAsync(int id)
        {
            return await _poliRepository.GetByIdAsync(id);
        }

        public async Task<bool> CreateAsync(PoliModel poli)
        {
            return await _poliRepository.CreateAsync(poli);
        }

        public async Task<bool> UpdateAsync(PoliModel poli)
        {
            return await _poliRepository.UpdateAsync(poli);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _poliRepository.DeleteAsync(id);
        }
    }
}
