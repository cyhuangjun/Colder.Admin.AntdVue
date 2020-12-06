using Coldairarrow.Entity.Base_Manage;
using Coldairarrow.Entity.Foundation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Coldairarrow.IBusiness.Core
{
    public interface ICacheDataBusiness
    {
        Task<List<Coin>> GetCoinsAsync();
        Task<Coin> GetCoinAsync(string coinId);
        Task<Coin> GetCoinByCodeAsync(string code);
        Task<Base_User> GetUserAsync(string userId);
        Task<Base_Department> GetTenantByUserIDAsync(string userId);
        Task<Base_Department> GetTenantAsync(string tenantId);

        Task<Wallet> GetWalletByAddress(string address);
        Task<Wallet> GetWallet(string tenantId, string clientUid, string coinId);
    }
}
