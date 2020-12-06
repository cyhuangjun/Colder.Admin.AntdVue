using Coldairarrow.Entity.Foundation;
using Coldairarrow.Util;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System;
using Coldairarrow.Entity.Response;

namespace Coldairarrow.Business.Foundation
{
    public interface ICoinConfigBusiness
    {
        Task<PageResult<CoinConfig>> GetDataListAsync(PageInput<ConditionDTO> input);
        Task<CoinConfig> GetTheDataAsync(string id);
        Task AddDataAsync(CoinConfig data);
        Task UpdateDataAsync(CoinConfig data);
        Task DeleteDataAsync(List<string> ids);
        Task<MinAmountViewDto> MinAmountAsync(string tenantId, string currency);
        Task<CoinConfig> GetEntityAsync(string tenantId, string currency);

        CoinConfig GetEntity(Expression<Func<CoinConfig, bool>> expression);
        Task<CoinConfig> GetEntityAsync(Expression<Func<CoinConfig, bool>> expression);
        List<CoinConfig> GetList(Expression<Func<CoinConfig, bool>> expression);
        Task<List<CoinConfig>> GetListAsync(Expression<Func<CoinConfig, bool>> expression);
        List<CoinConfig> GetList<TKey>(Expression<Func<CoinConfig, bool>> expression, Expression<Func<CoinConfig, TKey>> orderByDescending, int pageIndex, int pageSize = 20);
        Task<List<CoinConfig>> GetListAsync<TKey>(Expression<Func<CoinConfig, bool>> expression, Expression<Func<CoinConfig, TKey>> orderByDescending, int pageIndex, int pageSize = 20);
    }
}