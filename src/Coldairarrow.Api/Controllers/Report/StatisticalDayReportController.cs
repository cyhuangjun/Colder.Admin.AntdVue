using Coldairarrow.Business.Report;
using Coldairarrow.Entity.Report;
using Coldairarrow.Util;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coldairarrow.Api.Controllers.Report
{
    [Route("/Report/[controller]/[action]")]
    public class StatisticalDayReportController : BaseApiController
    {
        #region DI

        public StatisticalDayReportController(IStatisticalDayReportBusiness statisticalDayReportBus)
        {
            _statisticalDayReportBus = statisticalDayReportBus;
        }

        IStatisticalDayReportBusiness _statisticalDayReportBus { get; }

        #endregion

        #region 获取

        [HttpPost]
        public async Task<PageResult<StatisticalDayReport>> GetDataList(PageInput<ConditionDTO> input)
        {
            return await _statisticalDayReportBus.GetDataListAsync(input);
        }

        [HttpPost]
        public async Task<StatisticalDayReport> GetTheData(IdInputDTO input)
        {
            return await _statisticalDayReportBus.GetTheDataAsync(input.id);
        }

        #endregion

        #region 提交

        [HttpPost]
        public async Task SaveData(StatisticalDayReport data)
        {
            if (data.Id.IsNullOrEmpty())
            {
                InitEntity(data);

                await _statisticalDayReportBus.AddDataAsync(data);
            }
            else
            {
                await _statisticalDayReportBus.UpdateDataAsync(data);
            }
        }

        [HttpPost]
        public async Task DeleteData(List<string> ids)
        {
            await _statisticalDayReportBus.DeleteDataAsync(ids);
        }

        #endregion
    }
}