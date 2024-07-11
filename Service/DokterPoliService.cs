using Entity;
using Repository.Interface;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class DokterPoliService : IDokterPoliService
    {
        private readonly IDokterPoliRepository _dokterPoliRepository;

        public DokterPoliService(IDokterPoliRepository dokterPoliRepository)
        {
            _dokterPoliRepository = dokterPoliRepository;
        }
        public async Task<IEnumerable<DokterModel>> GetDokterByPoliAsync(int poliId)
        {
            return await _dokterPoliRepository.GetDokterByPoliAsync(poliId);
        }
    }
}
