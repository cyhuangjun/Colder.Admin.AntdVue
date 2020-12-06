using Coldairarrow.Business.Foundation;
using Coldairarrow.Entity.Foundation;
using Coldairarrow.Util;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coldairarrow.Api.Controllers.Foundation
{
    [Route("/Foundation/[controller]/[action]")]
    public class CoinConfigController : BaseApiController
    {
        #region DI

        public CoinConfigController(ICoinConfigBusiness coinConfigBus)
        {
            _coinConfigBus = coinConfigBus;
        }

        ICoinConfigBusiness _coinConfigBus { get; }

        #endregion

        #region 获取

        [HttpPost]
        public async Task<PageResult<CoinConfig>> GetDataList(PageInput<ConditionDTO> input)
        {
            return await _coinConfigBus.GetDataListAsync(input);
        }

        [HttpPost]
        public async Task<CoinConfig> GetTheData(IdInputDTO input)
        {
            return await _coinConfigBus.GetTheDataAsync(input.id);
        }

        [HttpPost]
        public async Task<List<SelectOption>> GetSelectOption()
        {
            var coinConfigs = await this._coinConfigBus.GetListAsync(e=>1==1);
            List<SelectOption> list = new List<SelectOption>();
            foreach (var aValue in coinConfigs)
            {
                list.Add(new SelectOption
                {
                    value = aValue.Id,
                    text = aValue.Caption
                });
            }
            return list;
        }
        #endregion

        #region 提交

        [HttpPost]
        public async Task SaveData(CoinConfig data)
        {
            if (data.Id.IsNullOrEmpty())
            {
                InitEntity(data);

                await _coinConfigBus.AddDataAsync(data);
            }
            else
            {
                await _coinConfigBus.UpdateDataAsync(data);
            }
        }

        [HttpPost]
        public async Task DeleteData(List<string> ids)
        {
            await _coinConfigBus.DeleteDataAsync(ids);
        }

        #endregion
    }
}