using Coldairarrow.Business.Transaction;
using Coldairarrow.Entity.Transaction;
using Coldairarrow.IBusiness;
using Coldairarrow.Util;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Coldairarrow.Api.Controllers.Transaction
{
    [Route("/Transaction/[controller]/[action]")]
    public class CoinTransactionInController : BaseApiController
    {
        #region DI

        public CoinTransactionInController(ICoinTransactionInBusiness coinTransactionInBus)
        {
            _coinTransactionInBus = coinTransactionInBus;
        }

        ICoinTransactionInBusiness _coinTransactionInBus { get; }

        #endregion

        #region 获取

        [HttpPost]
        public async Task<PageResult<CoinTransactionInReportDTO>> GetDataReportList(PageInput<CoinTransactionInInputDTO> input)
        {
            var op = HttpContext.RequestServices.GetService<IOperator>();
            if (!op.IsAdmin())
                input.Search.TenantId = op.TenantId;
            return await _coinTransactionInBus.GetReportDataListAsync(input);
        }

        [HttpPost]
        public async Task<CoinTransactionInReportDTO> GetTheDataReport(IdInputDTO input)
        {
            return await _coinTransactionInBus.GetTheReportDataAsync(input.id);
        }

        #endregion
    }
}