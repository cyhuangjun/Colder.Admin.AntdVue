using Coldairarrow.Entity.Report;
using Coldairarrow.Util;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System;

namespace Coldairarrow.Business.Report
{
    public interface IStatisticalDayReportBusiness
    {
        Task<PageResult<StatisticalDayReport>> GetDataListAsync(PageInput<ConditionDTO> input);
        Task<StatisticalDayReport> GetTheDataAsync(string id);
        Task AddDataAsync(StatisticalDayReport data);
        Task UpdateDataAsync(StatisticalDayReport data);
        Task DeleteDataAsync(List<string> ids);
        StatisticalDayReport GetEntity(Expression<Func<StatisticalDayReport, bool>> expression);
        Task<StatisticalDayReport> GetEntityAsync(Expression<Func<StatisticalDayReport, bool>> expression);
        List<StatisticalDayReport> GetList(Expression<Func<StatisticalDayReport, bool>> expression);
        Task<List<StatisticalDayReport>> GetListAsync(Expression<Func<StatisticalDayReport, bool>> expression);
        List<StatisticalDayReport> GetList<TKey>(Expression<Func<StatisticalDayReport, bool>> expression, Expression<Func<StatisticalDayReport, TKey>> orderByDescending, int pageIndex, int pageSize = 20);
        Task<List<StatisticalDayReport>> GetListAsync<TKey>(Expression<Func<StatisticalDayReport, bool>> expression, Expression<Func<StatisticalDayReport, TKey>> orderByDescending, int pageIndex, int pageSize = 20);
    }
}