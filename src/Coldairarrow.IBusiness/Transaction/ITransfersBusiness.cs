using Coldairarrow.Entity.Enum;
using Coldairarrow.Entity.Transaction;
using Coldairarrow.Util;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Coldairarrow.Business.Transaction
{
    public interface ITransfersBusiness
    {
        Task<PageResult<TransfersOutDTO>> GetDataListAsync(PageInput<TransfersInputDTO> input);
        Task<Transfers> GetTheDataAsync(string id);
        Task UpdateDataAsync(Transfers data);

        Transfers GetEntity(Expression<Func<Transfers, bool>> expression);
        Task<Transfers> GetEntityAsync(Expression<Func<Transfers, bool>> expression);
        List<Transfers> GetList(Expression<Func<Transfers, bool>> expression);
        Task<List<Transfers>> GetListAsync(Expression<Func<Transfers, bool>> expression);
        List<Transfers> GetList<TKey>(Expression<Func<Transfers, bool>> expression, Expression<Func<Transfers, TKey>> orderByDescending, int pageIndex, int pageSize = 20);
        Task<List<Transfers>> GetListAsync<TKey>(Expression<Func<Transfers, bool>> expression, Expression<Func<Transfers, TKey>> orderByDescending, int pageIndex, int pageSize = 20);
        Task<AjaxResult> Pass(string id);
        Task<AjaxResult> Deny(string id);
    }

    public class TransfersInputDTO
    {
        public TransfersStatus? Status { get; set; }
        public string Currency { get; set; }
        public string SearchKey { get; set; }
        public DateTime? startTime { get; set; }
        public DateTime? endTime { get; set; }
    }

    public class TransfersOutDTO
    {
        public string Id { set; get; }
        public TransfersStatus Status { get; set; }
        public string ClientOrderId { get; set; }
        public string Currency { set; get; }
        public decimal Amount { set; get; }
        public string AddressTo { set; get; }
        public decimal? HandlingFee { set; get; }
        public DateTime CreatedAt { set; get; }
        public DateTime? ApproveTime { set; get; }
        public string OrderDescription { set; get; }
        public string StatusStr
        {
            get
            {
                return Status.GetDescription();
            }
        }
        public string TXID { set; get; }
    }
}