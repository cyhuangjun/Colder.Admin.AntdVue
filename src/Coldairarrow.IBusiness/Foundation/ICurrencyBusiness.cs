using Coldairarrow.Entity.Foundation;
using Coldairarrow.Util;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coldairarrow.Business.Foundation
{
    public interface ICurrencyBusiness
    {
        Task<PageResult<Currency>> GetDataListAsync(PageInput<ConditionDTO> input);
        Task<Currency> GetTheDataAsync(string id);
        Task AddDataAsync(Currency data);
        Task UpdateDataAsync(Currency data);
        Task DeleteDataAsync(List<string> ids);
        Task<AjaxResult<IEnumerable<string>>> GetFiatCurrenciesAsync(); 
        Task<Currency> GetCurrencyByCodeAsync(string code);
    }
}