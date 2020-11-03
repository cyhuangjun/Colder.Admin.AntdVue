using Coldairarrow.Business.Foundation;
using Coldairarrow.Entity.Foundation;
using Coldairarrow.Util;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coldairarrow.Api.Controllers.Foundation
{
    [Route("/Foundation/[controller]/[action]")]
    public class CoinController : BaseApiController
    {
        #region DI

        public CoinController(ICoinBusiness coinBus)
        {
            _coinBus = coinBus;
        }

        ICoinBusiness _coinBus { get; }

        #endregion

        #region 获取

        [HttpPost]
        public async Task<PageResult<Coin>> GetDataList(PageInput<ConditionDTO> input)
        {
            return await _coinBus.GetDataListAsync(input);
        }

        [HttpPost]
        public async Task<Coin> GetTheData(IdInputDTO input)
        {
            return await _coinBus.GetTheDataAsync(input.id);
        }

        #endregion

        #region 提交

        [HttpPost]
        public async Task SaveData(Coin data)
        {
            if (data.Id.IsNullOrEmpty())
            {
                InitEntity(data);

                await _coinBus.AddDataAsync(data);
            }
            else
            {
                await _coinBus.UpdateDataAsync(data);
            }
        }

        [HttpPost]
        public async Task DeleteData(List<string> ids)
        {
            await _coinBus.DeleteDataAsync(ids);
        }

        #endregion
    }
}