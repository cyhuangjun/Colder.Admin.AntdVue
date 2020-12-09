using Coldairarrow.Business.Transaction;
using Coldairarrow.Entity.Transaction;
using Coldairarrow.Util;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coldairarrow.Api.Controllers.Transaction
{
    [Route("/Transaction/[controller]/[action]")]
    public class PaymentController : BaseApiController
    {
        #region DI

        public PaymentController(IPaymentBusiness paymentBus)
        {
            _paymentBus = paymentBus;
        }

        IPaymentBusiness _paymentBus { get; }

        #endregion

        #region 获取

        [HttpPost]
        public async Task<PageResult<Payment>> GetDataList(PageInput<ConditionDTO> input)
        {
            return await _paymentBus.GetDataListAsync(input);
        }

        [HttpPost]
        public async Task<Payment> GetTheData(IdInputDTO input)
        {
            return await _paymentBus.GetTheDataAsync(input.id);
        }

        #endregion

        #region 提交

        [HttpPost]
        public async Task SaveData(Payment data)
        {
            if (data.Id.IsNullOrEmpty())
            {
                InitEntity(data);

                await _paymentBus.AddDataAsync(data);
            }
            else
            {
                await _paymentBus.UpdateDataAsync(data);
            }
        }

        [HttpPost]
        public async Task DeleteData(List<string> ids)
        {
            await _paymentBus.DeleteDataAsync(ids);
        }

        #endregion
    }
}