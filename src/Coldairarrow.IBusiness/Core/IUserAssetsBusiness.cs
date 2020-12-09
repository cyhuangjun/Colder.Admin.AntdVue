using Coldairarrow.Entity.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coldairarrow.IBusiness.Core
{
    public interface IUserAssetsBusiness
    {
        decimal GetBalance(string userId, string coinCode);

        void UpdateAssets(AssetsChangeItemDTO assetsChangeItemDTO);
    }
}
