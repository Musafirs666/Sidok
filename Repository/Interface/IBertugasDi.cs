using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface IBertugasDi
    {
        Task<IEnumerable<BertugasDiModel>> GetAllBertugasDiAsync();
        Task<int> BertugasDiAsync(BertugasDiModel shop);
        Task<int> UpdateBertugasDiAsync(BertugasDiModel shop);
        Task<int> DeleteDokterAsync(int id);
    }
}
