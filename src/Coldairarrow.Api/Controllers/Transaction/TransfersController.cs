using Coldairarrow.Business.Transaction;
using Coldairarrow.Entity.Enum;
using Coldairarrow.Entity.Transaction;
using Coldairarrow.IBusiness;
using Coldairarrow.IBusiness.Core;
using Coldairarrow.Util;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Coldairarrow.Api.Controllers.Transaction
{
    [Route("/Transaction/[controller]/[action]")]
    public class TransfersController : BaseApiController
    {
        #region DI
        ITransfersBusiness _transfersBus;
        ICacheDataBusiness _cacheDataBusiness;
        public TransfersController(ITransfersBusiness transfersBus,
                                   ICacheDataBusiness cacheDataBusiness)
        {
            _transfersBus = transfersBus;
            _cacheDataBusiness = cacheDataBusiness;
        } 
        #endregion

        #region 获取

        [HttpPost]
        public async Task<PageResult<TransfersOutDTO>> GetDataList(PageInput<TransfersInputDTO> input)
        {
            var op = HttpContext.RequestServices.GetService<IOperator>();
            if (!op.IsAdmin())
                input.Search.TenantId = op.TenantId;
            return await _transfersBus.GetDataListAsync(input);
        }

        [HttpPost]
        public async Task<TransfersOutDTO> GetTheData(IdInputDTO input)
        {
            var theData = await _transfersBus.GetTheDataAsync(input.id);
            var coin = await this._cacheDataBusiness.GetCoinAsync(theData.CoinID);
            return new TransfersOutDTO()
            {
                AddressTo = theData.AddressTo,
                Amount = theData.Amount,
                ApproveTime = theData.ApproveTime,
                OrderId = theData.OrderId,
                CreatedAt = theData.CreatedAt,
                Currency = coin.Code,
                HandlingFee = theData.HandlingFee,
                Id = theData.Id,
                OrderDescription = theData.OrderDescription,
                Status = theData.Status
            };
        }

        [HttpPost]
        public async Task<PageResult<TransfersOutReportDTO>> GetDataReportList(PageInput<TransfersInputDTO> input)
        {
            var op = HttpContext.RequestServices.GetService<IOperator>();
            if (!op.IsAdmin())
                input.Search.TenantId = op.TenantId;
            return await _transfersBus.GetReportDataListAsync(input);
        }

        [HttpPost]
        public async Task<TransfersOutReportDTO> GetTheDataReport(IdInputDTO input)
        {
            return await _transfersBus.GetTheReportDataAsync(input.id); 
        }

        [HttpPost]
        public List<SelectOption> GetTransactionStatusList()
        {
            return EnumHelper.ToOptionListByDesc(typeof(TransfersStatus));
        }
        #endregion

        #region 提交

        [HttpPost]
        public async Task<AjaxResult> PassData(IdInputDTO input)
        {
            if (!input.id.IsNullOrEmpty())
            { 
              return await _transfersBus.Pass(input.id);
            }
            return this.Error();
        }

        [HttpPost]
        public async Task<AjaxResult> DenyData(IdInputDTO input)
        {
            if (!input.id.IsNullOrEmpty())
            {
                return await _transfersBus.Deny(input.id);
            }
            return this.Error();
        }

        #endregion
    }
}