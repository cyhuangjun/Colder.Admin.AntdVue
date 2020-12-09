using Coldairarrow.Entity.Foundation;
using Coldairarrow.Util;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coldairarrow.Business.Foundation
{
    public interface ICoinBusiness
    {
        Task<PageResult<Coin>> GetDataListAsync(PageInput<ConditionDTO> input);
        Task<Coin> GetTheDataAsync(string id);
        Task AddDataAsync(Coin data);
        Task UpdateDataAsync(Coin data);
        Task DeleteDataAsync(List<string> ids);
        Task<AjaxResult<IEnumerable<string>>> GetCryptoCurrenciesAsync();

        Task<Coin> GetCoinByCodeAsync(string code);

        Task<decimal> GetFeeRate(string userId, string coinId);
    }
}