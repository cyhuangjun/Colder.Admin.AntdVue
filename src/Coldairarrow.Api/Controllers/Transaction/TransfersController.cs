using Coldairarrow.Business.Transaction;
using Coldairarrow.Entity.Enum;
using Coldairarrow.Entity.Transaction;
using Coldairarrow.IBusiness.Core;
using Coldairarrow.Util;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            return await _transfersBus.GetDataListAsync(input);
        }

        [HttpPost]
        public async Task<TransfersOutDTO> GetTheData(IdInputDTO input)
        {
            var theData = await _transfersBus.GetTheDataAsync(input.id);
            return new TransfersOutDTO()
            {
                AddressTo = theData.AddressTo,
                Amount = theData.Amount,
                ApproveTime = theData.ApproveTime,
                ClientOrderId = theData.OrderId,
                CreatedAt = theData.CreatedAt,
                Currency = theData.Currency,
                HandlingFee = theData.HandlingFee,
                Id = theData.Id,
                OrderDescription = theData.OrderDescription,
                Status = theData.Status
            };
        }

        [HttpPost]
        public async Task<List<SelectOption>> GetCurrencyList()
        {
            var coins = await this._cacheDataBusiness.GetCoinsAsync();
            List<SelectOption> list = new List<SelectOption>();
            foreach (var aValue in coins)
            {
                list.Add(new SelectOption
                {
                    value = aValue.Id,
                    text = aValue.Code
                });
            }
            return list;
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