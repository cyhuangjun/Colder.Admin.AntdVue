﻿using Coldairarrow.Business.Foundation;
using Coldairarrow.Entity.Foundation;
using Coldairarrow.IBusiness.Core;
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
        ICacheDataBusiness _cacheDataBusiness;
        public CoinController(ICoinBusiness coinBus,
                                   ICacheDataBusiness cacheDataBusiness)
        {
            _coinBus = coinBus;
            _cacheDataBusiness = cacheDataBusiness;
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