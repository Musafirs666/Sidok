// SpesialisasiService.cs

using System.Collections.Generic;
using System.Threading.Tasks;
using Entity;
using Repository.Interface;
using Service.Interfaces;

namespace Service
{
    public class SpesialisasiService : ISpesialisasiService
    {
        private readonly ISpesialisasiRepository _spesialisasiRepository;

        public SpesialisasiService(ISpesialisasiRepository spesialisasiRepository)
        {
            _spesialisasiRepository = spesialisasiRepository;
        }

        public async Task<IEnumerable<SpesialisasiModel>> GetAllAsync()
        {
            return await _spesialisasiRepository.GetAllAsync();
        }
    }
}
