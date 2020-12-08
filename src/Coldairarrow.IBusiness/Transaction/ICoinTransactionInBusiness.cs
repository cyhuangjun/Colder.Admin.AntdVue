using Coldairarrow.Entity.Transaction;
using Coldairarrow.Util;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System;
using Coldairarrow.Entity.Enum;

namespace Coldairarrow.Business.Transaction
{
    public interface ICoinTransactionInBusiness
    {
        Task<PageResult<CoinTransactionIn>> GetDataListAsync(PageInput<ConditionDTO> input);
        Task<CoinTransactionIn> GetTheDataAsync(string id);
        Task AddDataAsync(CoinTransactionIn data);
        Task AddDataAsync(List<CoinTransactionIn> data);
        Task UpdateDataAsync(CoinTransactionIn data);
        Task UpdateDataAsync(List<CoinTransactionIn> data);
        Task DeleteDataAsync(List<string> ids);
        Task<PageResult<CoinTransactionInReportDTO>> GetReportDataListAsync(PageInput<CoinTransactionInInputDTO> input);
        Task<CoinTransactionInReportDTO> GetTheReportDataAsync(string id);

        CoinTransactionIn GetEntity(Expression<Func<CoinTransactionIn, bool>> expression);
        Task<CoinTransactionIn> GetEntityAsync(Expression<Func<CoinTransactionIn, bool>> expression);
        List<CoinTransactionIn> GetList(Expression<Func<CoinTransactionIn, bool>> expression);
        Task<List<CoinTransactionIn>> GetListAsync(Expression<Func<CoinTransactionIn, bool>> expression); 
        List<CoinTransactionIn> GetList<TKey>(Expression<Func<CoinTransactionIn, bool>> expression, Expression<Func<CoinTransactionIn, TKey>> orderByDescending, int pageIndex, int pageSize = 20); 
        Task<List<CoinTransactionIn>> GetListAsync<TKey>(Expression<Func<CoinTransactionIn, bool>> expression, Expression<Func<CoinTransactionIn, TKey>> orderByDescending, int pageIndex, int pageSize = 20);

        Task<AjaxResult> SyncCoinTransactionIn(List<CoinTransactionIn> coinTransactionList, List<CoinContranctTransactionIn> coinContranctTransactionIns);
    }

    public class CoinTransactionInInputDTO
    {
        public string Id { set; get; }
        public string TenantId { set; get; }
        public TransactionStatus? Status { get; set; }
        public string CoinID { get; set; }
        public string SearchKey { get; set; }
        public DateTime? startTime { get; set; }
        public DateTime? endTime { get; set; }
    }

    [Map(typeof(CoinTransactionIn))]
    public class CoinTransactionInReportDTO : CoinTransactionIn
    {
        public string CUID { set; get; }
        public string Tenant { set; get; }
        public string Currency { set; get; }
        public string StatusStr
        {
            get
            {
                return Status.GetDescription();
            }
        }
        public string MinefeeCurrency { set; get; }
        public string CallBackStatusStr
        {
            get
            {
                return CallBackStatus?.GetDescription();
            }
        }
        public string MoveStatusStr
        {
            get
            {
                return MoveStatus.GetDescription();
            }
        }

        public decimal Minefee
        {
            get
            {
                return this.MoveUserMinefee + MoveSysMinefee;
            }
        }
    }
}