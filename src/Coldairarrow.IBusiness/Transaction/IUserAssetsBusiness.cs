﻿using Coldairarrow.Entity.DTO;
using Coldairarrow.Entity.Transaction;
using Coldairarrow.Util;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coldairarrow.Business.Transaction
{
    public interface IUserAssetsBusiness
    {
        Task<decimal> GetBalance(string userId, string coinCode);

        Task<AjaxResult> UpdateAssets(params AssetsChangeItemDTO[] assetsChangeItems);
    }
}