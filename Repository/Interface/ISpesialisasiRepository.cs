using Entity;

namespace Repository.Interface
{
    public interface ISpesialisasiRepository
    {
        Task<IEnumerable<SpesialisasiModel>> GetAllAsync();
    }
}
