using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IOpensearchDokterService
    {
        Task<List<DokterOpensearchModel>> SearchDokterAsync(string searchTerm);
    }
}
