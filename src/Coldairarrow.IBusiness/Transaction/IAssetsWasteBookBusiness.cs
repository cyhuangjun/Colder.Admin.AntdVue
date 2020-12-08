using Coldairarrow.Entity.Transaction;
using Coldairarrow.Util;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Coldairarrow.Business.Transaction
{
    public interface IAssetsWasteBookBusiness
    {
        Task<PageResult<AssetsWasteBookOutDTO>> GetDataListAsync(PageInput<AssetsWasteBookInputDTO> input);
        Task<AssetsWasteBook> GetTheDataAsync(string id);
        Task AddDataAsync(AssetsWasteBook data);

        AssetsWasteBook GetEntity(Expression<Func<AssetsWasteBook, bool>> expression);
        Task<AssetsWasteBook> GetEntityAsync(Expression<Func<AssetsWasteBook, bool>> expression);
        List<AssetsWasteBook> GetList(Expression<Func<AssetsWasteBook, bool>> expression);
        Task<List<AssetsWasteBook>> GetListAsync(Expression<Func<AssetsWasteBook, bool>> expression);
        List<AssetsWasteBook> GetList<TKey>(Expression<Func<AssetsWasteBook, bool>> expression, Expression<Func<AssetsWasteBook, TKey>> orderByDescending, int pageIndex, int pageSize = 20);
        Task<List<AssetsWasteBook>> GetListAsync<TKey>(Expression<Func<AssetsWasteBook, bool>> expression, Expression<Func<AssetsWasteBook, TKey>> orderByDescending, int pageIndex, int pageSize = 20);
    }

    public class AssetsWasteBookInputDTO
    {
        public string TenantId { get; set; }
        public string CoinID { get; set; }
    }

    [Map(typeof(AssetsWasteBook))]
    public class AssetsWasteBookOutDTO : AssetsWasteBook
    {
        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { set; get; }
        /// <summary>
        /// 商户
        /// </summary>
        public string Tenant { set; get; }

        public string AssetsWasteBookTypeStr 
        {
            get
            {
                return this.AssetsWasteBookType.GetDescription();
            }
        }
    }
}