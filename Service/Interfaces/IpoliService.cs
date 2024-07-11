using Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IPoliService
    {
        Task<IEnumerable<PoliModel>> GetAllAsync();
        Task<PoliModel> GetByIdAsync(int id);
        Task<bool> CreateAsync(PoliModel poli);
        Task<bool> UpdateAsync(PoliModel poli);
        Task<bool> DeleteAsync(int id);
    }
}
