using Entity;

namespace Repository.Interface
{
    public interface IPoliRepository
    {
        Task<IEnumerable<PoliModel>> GetAllAsync();
        Task<bool> UpdateAsync(PoliModel poli);
        Task<bool> DeleteAsync(int id);
        Task<bool> CreateAsync(PoliModel poli);
        Task<PoliModel> GetByIdAsync(int id);

    }
}
