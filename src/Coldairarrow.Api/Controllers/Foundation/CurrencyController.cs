using Coldairarrow.Business.Foundation;
using Coldairarrow.Entity.Foundation;
using Coldairarrow.Util;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coldairarrow.Api.Controllers.Foundation
{
    [Route("/Foundation/[controller]/[action]")]
    public class CurrencyController : BaseApiController
    {
        #region DI

        public CurrencyController(ICurrencyBusiness currencyBus)
        {
            _currencyBus = currencyBus;
        }

        ICurrencyBusiness _currencyBus { get; }

        #endregion

        #region 获取

        [HttpPost]
        public async Task<PageResult<Currency>> GetDataList(PageInput<ConditionDTO> input)
        {
            return await _currencyBus.GetDataListAsync(input);
        }

        [HttpPost]
        public async Task<Currency> GetTheData(IdInputDTO input)
        {
            return await _currencyBus.GetTheDataAsync(input.id);
        }

        #endregion

        #region 提交

        [HttpPost]
        public async Task SaveData(Currency data)
        {
            if (data.Id.IsNullOrEmpty())
            {
                InitEntity(data);

                await _currencyBus.AddDataAsync(data);
            }
            else
            {
                await _currencyBus.UpdateDataAsync(data);
            }
        }

        [HttpPost]
        public async Task DeleteData(List<string> ids)
        {
            await _currencyBus.DeleteDataAsync(ids);
        }

        #endregion
    }
}