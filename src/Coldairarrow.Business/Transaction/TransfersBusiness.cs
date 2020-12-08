using CCPP.Cryptocurrency.Common;
using Coldairarrow.Entity;
using Coldairarrow.Entity.Base_Manage;
using Coldairarrow.Entity.DTO;
using Coldairarrow.Entity.Enum;
using Coldairarrow.Entity.Foundation;
using Coldairarrow.Entity.Transaction;
using Coldairarrow.IBusiness;
using Coldairarrow.IBusiness.Core;
using Coldairarrow.Util;
using EFCore.Sharding;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Coldairarrow.Business.Transaction
{
    public class TransfersBusiness : BaseBusiness<Transfers>, ITransfersBusiness, ITransientDependency
    {
        static Dictionary<string, DateTime> auditOrders = new Dictionary<string, DateTime>();
        private readonly ICacheDataBusiness _cacheDataBusiness;
        private readonly ICryptocurrencyBusiness _cryptocurrencyBusiness;
        private readonly IOperator _operator;
        private readonly IAssetsBusiness _userAssetsBusiness;
        public TransfersBusiness(IDbAccessor db,
                                IOperator @operator,
                                IAssetsBusiness userAssetsBusiness,
                                ICacheDataBusiness cacheDataBusiness)
            : base(db)
        {
            this._operator = @operator;
            this._userAssetsBusiness = userAssetsBusiness;
            this._cacheDataBusiness = cacheDataBusiness;
        } 

        #region 外部接口

        public async Task<PageResult<TransfersOutDTO>> GetDataListAsync(PageInput<TransfersInputDTO> input)
        {
            Expression<Func<Transfers, Coin, Base_Department, TransfersOutDTO>> select = (a, b, c) => new TransfersOutDTO
            {
                Currency = b.Code,
                Tenant = c.Name
            };
            select = select.BuildExtendSelectExpre();
            var q = from a in this.Db.GetIQueryable<Transfers>().AsExpandable()
                    join b in Db.GetIQueryable<Coin>() on a.CoinID equals b.Id into ab
                    from b in ab.DefaultIfEmpty()
                    join c in Db.GetIQueryable<Base_Department>() on a.TenantId equals c.Id into ac
                    from c in ac.DefaultIfEmpty()
                    select @select.Invoke(a, b, c); 
            //筛选
            var where = LinqHelper.True<TransfersOutDTO>();
            var search = input.Search;
            if (!string.IsNullOrEmpty(search.Currency))
                where = where.And(x => x.Currency == search.Currency);
            if (search.Status.HasValue)
                where = where.And(x => x.Status == search.Status.Value);
            if (!search.SearchKey.IsNullOrEmpty())
                where = where.And(x => x.OrderDescription.Contains(search.SearchKey) || x.OrderId.Contains(search.SearchKey) || x.Id.Contains(search.SearchKey));
            if (!search.startTime.IsNullOrEmpty())
                where = where.And(x => x.CreatedAt >= search.startTime);
            if (!search.endTime.IsNullOrEmpty())
                where = where.And(x => x.CreatedAt < search.endTime);
            if (!string.IsNullOrEmpty(search.TenantId))
                where = where.And(x=>x.TenantId == search.TenantId);
            //if (!string.IsNullOrEmpty(search.Id))
            //    where = where.And(x => x.Id == search.Id);
            return await q.Where(where).GetPageResultAsync(input);
        }

        public async Task<Transfers> GetTheDataAsync(string id)
        {
            return await GetEntityAsync(id);
        }

        public async Task<PageResult<TransfersOutReportDTO>> GetReportDataListAsync(PageInput<TransfersInputDTO> input)
        {
            Expression<Func<Transfers, Coin, Base_Department, CoinTransactionOut, Coin, TransfersOutReportDTO>> select = (a, b, c, d, e) => new TransfersOutReportDTO
            {
                Currency = b.Code,
                Tenant = c.Name,
                Minefee = d.Minefee,
                MinefeeCurrency = e.Code,
                AddressTag = d.AddressTag
            };
            select = select.BuildExtendSelectExpre();
            var q = from a in this.Db.GetIQueryable<Transfers>().AsExpandable()
                    join b in Db.GetIQueryable<Coin>() on a.CoinID equals b.Id into ab
                    from b in ab.DefaultIfEmpty()
                    join c in Db.GetIQueryable<Base_Department>() on a.TenantId equals c.Id into ac
                    from c in ac.DefaultIfEmpty()
                    join d in Db.GetIQueryable<CoinTransactionOut>() on a.TransactionOutID equals d.Id into ad
                    from d in ad.DefaultIfEmpty()
                    join e in Db.GetIQueryable<Coin>() on d.CoinID equals e.Id into de
                    from e in de.DefaultIfEmpty()
                    select @select.Invoke(a, b, c, d, e);
            //筛选
            var where = LinqHelper.True<TransfersOutReportDTO>();
            var search = input.Search;
            if (!string.IsNullOrEmpty(search.Currency))
                where = where.And(x => x.Currency == search.Currency);
            if (search.Status.HasValue)
                where = where.And(x => x.Status == search.Status.Value);
            if (!search.SearchKey.IsNullOrEmpty())
                where = where.And(x => x.OrderDescription.Contains(search.SearchKey) || x.OrderId.Contains(search.SearchKey) || x.Id.Contains(search.SearchKey));
            if (!search.startTime.IsNullOrEmpty())
                where = where.And(x => x.CreatedAt >= search.startTime);
            if (!search.endTime.IsNullOrEmpty())
                where = where.And(x => x.CreatedAt < search.endTime);
            if (!string.IsNullOrEmpty(search.TenantId))
                where = where.And(x => x.TenantId == search.TenantId);
            if (!string.IsNullOrEmpty(search.Id))
                where = where.And(x => x.Id == search.Id);
            return await q.Where(where).GetPageResultAsync(input);
        }

        public async Task<TransfersOutReportDTO> GetTheReportDataAsync(string id)
        {
            return (await this.GetReportDataListAsync(new PageInput<TransfersInputDTO>() { Search = new TransfersInputDTO() { Id = id } })).Data.FirstOrDefault(e=>e.Id == id);
        }

        public async Task UpdateDataAsync(Transfers data)
        {
            await UpdateAsync(data);
        }

        [Transactional]
        public async Task<AjaxResult> Pass(string id)
        {
            var result = new AjaxResult();
            if (auditOrders.ContainsKey(id))
            {
                var dt = auditOrders[id];
                if (DateTime.Now.Subtract(dt).Hours < 1)
                {
                    result.Success = false;
                    result.Msg = "重复提交,请稍候再试.";
                    result.ErrorCode = ErrorCodeDefine.UnknownError;
                    return result;
                }
                auditOrders[id] = DateTime.Now;
            }
            else
            {
                auditOrders.Add(id, DateTime.Now);
            }
            var transfers = await this.GetTheDataAsync(id);
            if (transfers?.Status != TransfersStatus.Waiting)
            {
                result.Success = false;
                result.Msg = "非法操作!";
                result.ErrorCode = ErrorCodeDefine.IllegalOperation;
                return result;
            }

            var transactionOut = await this.Db.GetEntityAsync<CoinTransactionOut>(transfers.TransactionOutID);
            if (transactionOut?.Status != TransactionStatus.Apply)
            {
                result.Success = false;
                result.Msg = "非法操作!";
                result.ErrorCode = ErrorCodeDefine.IllegalOperation;
                return result;
            }

            var coin = await this._cacheDataBusiness.GetCoinAsync(transactionOut.CoinID);
            if(!coin.IsSupportWallet)
            {
                result.Success = false;
                result.Msg = "钱包不可用";
                result.ErrorCode = ErrorCodeDefine.WalletMaintenancing;
                return result;
            }
            var cryptocurrencyProvider = await _cryptocurrencyBusiness.GetCryptocurrencyProviderAsync(coin);
            var balanceResult = cryptocurrencyProvider.GetAllBalance(coin.MinConfirms);
            if (!string.IsNullOrEmpty(balanceResult.Error))
            {
                result.Success = false;
                result.Msg = $"钱包不可用, {balanceResult.Error}.";
                result.ErrorCode = ErrorCodeDefine.WalletMaintenancing;
                return result;
            }
            else if(balanceResult.Result < transactionOut.Amount)
            {
                result.Success = false;
                result.Msg = $"钱包不可用余额不足.";
                result.ErrorCode = ErrorCodeDefine.WalletBalanceNotEnought;
                return result;
            } 

            AjaxResult assetsResult = await UpdateAssets(transfers, transactionOut);
            if (!assetsResult.Success)
            {
                result.Success = false;
                result.Msg = assetsResult.Msg;
                result.ErrorCode = assetsResult.ErrorCode;
                return result;
            }

            var walletSendResult = SendTo(cryptocurrencyProvider, transactionOut);

            await UpdateData(transfers, transactionOut, walletSendResult);

            if (!walletSendResult.Success)
            {
                result.Success = false;
                result.Msg = walletSendResult.Msg;
                result.ErrorCode = walletSendResult.ErrorCode;
            }
            else
            {
                result.Success = true;
                result.ErrorCode = ErrorCodeDefine.Success;
            }
            return result;
        } 

        [Transactional]
        public async Task<AjaxResult> Deny(string id)
        {
            var result = new AjaxResult();
            var transfers = await this.GetTheDataAsync(id);
            if (transfers?.Status != TransfersStatus.Waiting)
            {
                result.Success = false;
                result.Msg = "非法操作!";
                result.ErrorCode = ErrorCodeDefine.IllegalOperation;
                return result;
            }

            var transactionOut = await this.Db.GetEntityAsync<CoinTransactionOut>(transfers.TransactionOutID);
            if (transactionOut?.Status != TransactionStatus.Apply)
            {
                result.Success = false;
                result.Msg = "非法操作!";
                result.ErrorCode = ErrorCodeDefine.IllegalOperation;
                return result;
            }

            var fee = transfers.HandlingFee ?? 0;
            var assetsChangeItemDTO = new AssetsChangeItemDTO()
            {
                CoinID = transactionOut.CoinID,
                AssetsWasteBookType = AssetsWasteBookType.CoinOut,
                ChangeAvailableAmount = (transactionOut.Amount + fee),
                ChangeFrozenAmount = -1 * (transactionOut.Amount + fee),
                FeeAmount = -fee,
                RelateID = transfers.Id,
                TenantId = transfers.TenantId
            };
            var assetsResult = await this._userAssetsBusiness.UpdateAssets(assetsChangeItemDTO);
            if (!assetsResult.Success)
            {
                result.Success = false;
                result.Msg = assetsResult.Msg;
                result.ErrorCode = assetsResult.ErrorCode;
                return result;
            }

            transfers.Status = TransfersStatus.Disallowance;
            transfers.UpdatedAt = DateTime.Now;
            transfers.ApproveTime = DateTime.Now;
            transfers.LastUpdateUserID = this._operator.UserId;
            transfers.ApproverID = this._operator.UserId;
            await this.Db.UpdateAsync(transfers);

            transactionOut.Status = TransactionStatus.ApprovalReject;
            transactionOut.LastUpdateTime = DateTime.Now;
            transactionOut.ApproveTime = DateTime.Now;
            transactionOut.LastUpdateUserID = this._operator.UserId;
            transactionOut.ApproverID = this._operator.UserId;
            await this.Db.UpdateAsync(transactionOut);
            result.Success = true;
            result.ErrorCode = ErrorCodeDefine.Success;
            return result;
        }

        #endregion

        #region 私有成员
        private AjaxResult<string> SendTo(ICryptocurrencyProvider cryptocurrencyProvider, CoinTransactionOut transactionOut)
        {
            var result = new AjaxResult<string>();
            var sendResult = cryptocurrencyProvider.SendTransaction(new SendCoinData()
            {
                Amount = transactionOut.Amount,
                ToCoinAddress = transactionOut.Address,
                ToCoinAddressTag = transactionOut.AddressTag,
            });
            if (string.IsNullOrEmpty(sendResult.Error))
            {
                result.Data = sendResult.Result;
                result.Success = true;
                result.ErrorCode = ErrorCodeDefine.Success;
                return result;
            }
            else
            {
                result.Success = false;
                result.ErrorCode = ErrorCodeDefine.WalletServceError;
                result.Msg = sendResult.Error;
                return result; 
            }
        }

        private async Task UpdateData(Transfers transfers, CoinTransactionOut transactionOut, AjaxResult<string> walletSendResult)
        {
            transfers.TXID = walletSendResult.Data;
            transfers.Status = (walletSendResult.Success ? TransfersStatus.Confirming : TransfersStatus.Failed);
            transfers.UpdatedAt = DateTime.Now;
            transfers.ApproveTime = DateTime.Now;
            transfers.LastUpdateUserID = this._operator.UserId;
            transfers.ApproverID = this._operator.UserId;
            await this.Db.UpdateAsync(transfers);

            transactionOut.TXID = walletSendResult.Data;
            transactionOut.Status = (walletSendResult.Success ? TransactionStatus.WaitConfirm : TransactionStatus.ProcessFail);
            transactionOut.LastUpdateTime = DateTime.Now;
            transactionOut.ApproveTime = DateTime.Now;
            transactionOut.LastUpdateUserID = this._operator.UserId;
            transactionOut.ApproverID = this._operator.UserId;
            await this.Db.UpdateAsync(transactionOut);
        }

        private async Task<AjaxResult> UpdateAssets(Transfers transfers, CoinTransactionOut transactionOut)
        {
            var fee = transfers.HandlingFee ?? 0;
            var assetsChangeItemDTO = new AssetsChangeItemDTO()
            {
                CoinID = transactionOut.CoinID,
                AssetsWasteBookType = AssetsWasteBookType.CoinOut,
                ChangeAvailableAmount = 0,
                ChangeFrozenAmount = -1 * (transactionOut.Amount + fee),
                FeeAmount = 0,
                RelateID = transfers.Id,
                TenantId = transfers.TenantId
            };
            var assetsResult = await this._userAssetsBusiness.UpdateAssets(assetsChangeItemDTO);
            return assetsResult;
        }
        #endregion
    }
}