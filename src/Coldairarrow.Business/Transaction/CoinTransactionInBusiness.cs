using Coldairarrow.Entity.Base_Manage;
using Coldairarrow.Entity.Foundation;
using Coldairarrow.Entity.Transaction;
using Coldairarrow.Util;
using EFCore.Sharding;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading.Tasks; 

namespace Coldairarrow.Business.Transaction
{
    public class CoinTransactionInBusiness : BaseBusiness<CoinTransactionIn>, ICoinTransactionInBusiness, ITransientDependency
    {
        public CoinTransactionInBusiness(IDbAccessor db)
            : base(db)
        {
        }

        #region 外部接口
        public async Task<PageResult<CoinTransactionInReportDTO>> GetReportDataListAsync(PageInput<CoinTransactionInInputDTO> input)
        {
            Expression<Func<CoinTransactionIn, Coin, Base_Department, Coin, Wallet, CoinTransactionInReportDTO>> select = (a, b, c, d, e) => new CoinTransactionInReportDTO
            {
                Currency = b.Code,
                Tenant = c.Name,
                MinefeeCurrency = d.Code,
                CUID = e.UID
            };
            select = select.BuildExtendSelectExpre();
            var q = from a in this.Db.GetIQueryable<CoinTransactionIn>().AsExpandable()
                    join b in Db.GetIQueryable<Coin>() on a.CoinID equals b.Id into ab
                    from b in ab.DefaultIfEmpty()
                    join c in Db.GetIQueryable<Base_Department>() on a.TenantId equals c.Id into ac
                    from c in ac.DefaultIfEmpty()                   
                    join d in Db.GetIQueryable<Coin>() on a.MinefeeCoinID equals d.Id into ad
                    from d in ad.DefaultIfEmpty()
                    join e in Db.GetIQueryable<Wallet>() on a.Address equals e.Address into ae
                    from e in ae.DefaultIfEmpty()
                    select @select.Invoke(a, b, c, d, e);
            //筛选
            var where = LinqHelper.True<CoinTransactionInReportDTO>();
            var search = input.Search;
            if (!string.IsNullOrEmpty(search.CoinID))
                where = where.And(x => x.CoinID == search.CoinID);
            if (search.Status.HasValue)
                where = where.And(x => x.Status == search.Status.Value);
            if (!search.SearchKey.IsNullOrEmpty())
                where = where.And(x => x.CUID.Contains(search.SearchKey) || x.FromAddress.Contains(search.SearchKey) || x.Address.Contains(search.SearchKey) || x.Tenant.Contains(search.SearchKey) || x.Id.Contains(search.SearchKey));
            if (!search.startTime.IsNullOrEmpty())
                where = where.And(x => x.CreateTime >= search.startTime);
            if (!search.endTime.IsNullOrEmpty())
                where = where.And(x => x.CreateTime < search.endTime);
            if (!string.IsNullOrEmpty(search.TenantId))
                where = where.And(x => x.TenantId == search.TenantId);
            if (!string.IsNullOrEmpty(search.Id))
                where = where.And(x => x.Id == search.Id);
            return await q.Where(where).GetPageResultAsync(input);
        }

        public async Task<CoinTransactionInReportDTO> GetTheReportDataAsync(string id)
        {
            return (await this.GetReportDataListAsync(new PageInput<CoinTransactionInInputDTO>() { Search = new CoinTransactionInInputDTO() { Id = id } })).Data.FirstOrDefault(e => e.Id == id);
        }

        public async Task<PageResult<CoinTransactionIn>> GetDataListAsync(PageInput<ConditionDTO> input)
        {
            var q = GetIQueryable();
            var where = LinqHelper.True<CoinTransactionIn>();
            var search = input.Search;

            //筛选
            if (!search.Condition.IsNullOrEmpty() && !search.Keyword.IsNullOrEmpty())
            {
                var newWhere = DynamicExpressionParser.ParseLambda<CoinTransactionIn, bool>(
                    ParsingConfig.Default, false, $@"{search.Condition}.Contains(@0)", search.Keyword);
                where = where.And(newWhere);
            }

            return await q.Where(where).GetPageResultAsync(input);
        }

        public async Task<CoinTransactionIn> GetTheDataAsync(string id)
        {
            return await GetEntityAsync(id);
        }

        public async Task AddDataAsync(CoinTransactionIn data)
        {
            await InsertAsync(data);
        }

        public async Task AddDataAsync(List<CoinTransactionIn> data)
        {
            await InsertAsync(data);
        }

        public async Task UpdateDataAsync(CoinTransactionIn data)
        {
            await UpdateAsync(data);
        }

        public async Task UpdateDataAsync(List<CoinTransactionIn> data)
        {
            await UpdateAsync(data); 
        }

        public async Task DeleteDataAsync(List<string> ids)
        {
            await DeleteAsync(ids);
        }

        public async Task<AjaxResult> SyncCoinTransactionIn(List<CoinTransactionIn> coinTransactionList, List<CoinContranctTransactionIn> coinContranctTransactionIns)
        {
            if (coinTransactionList == null && coinContranctTransactionIns == null ||
                ((coinTransactionList != null && !coinTransactionList.Any()) && (coinContranctTransactionIns != null && !coinContranctTransactionIns.Any())))
            {
                return this.Success();
            }
            List<string> transactionIDs = new List<string>();
            if (coinTransactionList != null && coinTransactionList.Any())
            {
                var txList = coinTransactionList.Select(f => f.TXID).Distinct().ToList();
                if (txList != null && txList.Any())
                {
                    transactionIDs.AddRange(txList);
                }
            }
            if (coinContranctTransactionIns != null && coinContranctTransactionIns.Any())
            {
                var txIDs = coinContranctTransactionIns.Select(f => f.TXID).Distinct().ToList();
                if (txIDs != null && txIDs.Any())
                {
                    transactionIDs.AddRange(txIDs);
                }
                var contranctTransactionInList = await this.Db.GetIQueryable<CoinContranctTransactionIn>().Where(f => txIDs.Contains(f.TXID)).ToListAsync();
                if (contranctTransactionInList != null && contranctTransactionInList.Any())
                {
                    var orgTxIds = contranctTransactionInList.Select(f => f.TXID).Distinct().ToList();
                    coinContranctTransactionIns = coinContranctTransactionIns.Where(f => !orgTxIds.Contains(f.TXID)).ToList();
                }
            }
            var moveTokenRecords = await this.GetListAsync(f => transactionIDs.Contains(f.SysToUserTXID));
            if (moveTokenRecords != null && moveTokenRecords.Any())
            {
                var moveTokenTxIds = moveTokenRecords.Select(f => f.SysToUserTXID).Distinct().ToList();
                if (coinTransactionList != null && coinTransactionList.Any())
                {
                    coinTransactionList = coinTransactionList.Where(f => !moveTokenTxIds.Contains(f.TXID)).Distinct().ToList();
                }
                if (coinContranctTransactionIns != null && coinContranctTransactionIns.Any())
                {
                    coinContranctTransactionIns = coinContranctTransactionIns.Where(f => !moveTokenTxIds.Contains(f.TXID)).Distinct().ToList();
                }
            } 
            if (coinTransactionList != null && coinTransactionList.Any())
                await this.AddDataAsync(coinTransactionList);
            if (coinContranctTransactionIns != null && coinContranctTransactionIns.Any())
                this.Db.Insert(coinContranctTransactionIns);
            return Success();
        }

        #endregion

        #region 私有成员

        #endregion
    }
}