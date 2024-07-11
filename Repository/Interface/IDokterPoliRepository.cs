using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity;

namespace Repository.Interface
{
    public interface IDokterPoliRepository
    {
        Task<IEnumerable<DokterModel>> GetDokterByPoliAsync(int poliId);
    }
}
