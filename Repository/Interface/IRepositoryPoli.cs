using Entity;

namespace Repository.Interface
{
    public interface IRepositoryPoli
    {
        Task<IEnumerable<PoliModel>> GetAllPoliAsync();
        Task<PoliModel> GetPoliByIdAsync(int id);
        Task<int> AddPoliAsync(PoliModel shop);
        Task<int> UpdatePoliAsync(PoliModel shop);
        Task<int> DeletePoliAsync(int id);
    }
}
