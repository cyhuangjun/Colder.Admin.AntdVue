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
        Task<Base_User> GetUserAsync(string userId);

        Task<Wallet> GetWalletByAddress(string address);
        Task<Wallet> GetWallet(string userId, string clientUid, string coinId);
    }
}
