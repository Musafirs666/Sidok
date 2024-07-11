using Entity;

namespace Service.Interfaces
{
    public interface ISpesialisasiService
    {
        Task<IEnumerable<SpesialisasiModel>> GetAllAsync();
    }
}
