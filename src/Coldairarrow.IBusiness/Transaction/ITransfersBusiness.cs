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
        Task<PageResult<TransfersOutReportDTO>> GetReportDataListAsync(PageInput<TransfersInputDTO> input);
        Task<TransfersOutReportDTO> GetTheReportDataAsync(string id);
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
        public string Id { set; get; }
        public string TenantId { set; get; }
        public TransfersStatus? Status { get; set; }
        public string Currency { get; set; }
        public string SearchKey { get; set; }
        public DateTime? startTime { get; set; }
        public DateTime? endTime { get; set; }
    }

    [Map(typeof(Transfers))]
    public class TransfersOutDTO : Transfers
    { 
        public string Tenant { set; get; }
        public string Currency { set; get; } 
        public string StatusStr
        {
            get
            {
                return Status.GetDescription();
            }
        }
    }

    [Map(typeof(Transfers))]
    public class TransfersOutReportDTO : TransfersOutDTO
    {
        public decimal? Minefee { set; get; }
        public string MinefeeCurrency { set; get; }
        public string AddressTag { set; get; }
        public string CallBackStatusStr
        {
            get
            {
                return CallBackStatus?.GetDescription();
            }
        }
    }
}