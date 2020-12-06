using AutoMapper;
using CacheManager.Core;
using Coldairarrow.Entity;
using Coldairarrow.Entity.Base_Manage;
using Coldairarrow.Entity.Foundation;
using Coldairarrow.IBusiness.Core;
using Coldairarrow.Util;
using EFCore.Sharding;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Coldairarrow.Business.Base_Manage
{
    public class Base_DepartmentBusiness : BaseBusiness<Base_Department>, IBase_DepartmentBusiness, ITransientDependency
    {
        readonly ICacheDataBusiness _cacheDataBusiness;
        readonly IMapper _mapper;
        ICacheManager<object> _cache;
        public Base_DepartmentBusiness(IDbAccessor db,
                                        ICacheManager<object> cache,
                                        IMapper mapper,
                                        ICacheDataBusiness cacheDataBusiness)
            : base(db)
        {
            _cache = cache;
            _mapper = mapper;
            _cacheDataBusiness = cacheDataBusiness;
        }

        #region 外部接口

        public async Task<List<Base_DepartmentTreeDTO>> GetTreeDataListAsync(DepartmentsTreeInputDTO input)
        {
            var where = LinqHelper.True<Base_Department>();
            if (!input.parentId.IsNullOrEmpty())
                where = where.And(x => x.ParentId == input.parentId);

            var list = await GetIQueryable().Where(where).ToListAsync();
            var treeList = list
                .Select(x => new Base_DepartmentTreeDTO
                {
                    Id = x.Id,
                    ParentId = x.ParentId,
                    Text = x.Name,
                    Value = x.Id,
                    Code = x.Code,
                    IsFrozen = ((x.IsFrozen ?? false) ? "是" : "否")
                }).ToList();
            return TreeHelper.BuildTree(treeList);
        }

        public async Task<List<string>> GetChildrenIdsAsync(string departmentId)
        {
            var allNode = await GetIQueryable().Select(x => new TreeModel
            {
                Id = x.Id,
                ParentId = x.ParentId,
                Text = x.Name,
                Value = x.Id
            }).ToListAsync();

            var children = TreeHelper
                .GetChildren(allNode, allNode.Where(x => x.Id == departmentId).FirstOrDefault(), true)
                .Select(x => x.Id)
                .ToList();

            return children;
        }

        public async Task<Base_DepartmentDTO> GetTheDataAsync(string id)
        {
            Expression<Func<Base_Department, Base_DepartmentDTO>> select = (a) => new Base_DepartmentDTO
            {
            };
            select = select.BuildExtendSelectExpre();
            var q = from a in GetIQueryable().AsExpandable()
                    select @select.Invoke(a);
            q = q.Where(x => x.Id == id);
            var result = await q.FirstOrDefaultAsync(); 
            return result;
        }

        [Transactional]
        [DataRepeatValidate(new string[] { "Name" }, new string[] { "部门名" })]
        [DataAddLog(UserLogType.DepartmentManagement, "Name", "部门名")]
        public async Task AddDataAsync(DepartmentEditInputDTO newData)
        { 
            await InsertAsync(_mapper.Map<Base_Department>(newData));
            await SaveCoinConfigAsync(newData.Id, newData.coinConfigList);
        }

        [Transactional]
        [DataRepeatValidate(new string[] { "Name" }, new string[] { "部门名" })]
        [DataEditLog(UserLogType.DepartmentManagement, "Name", "部门名")]
        public async Task UpdateDataAsync(DepartmentEditInputDTO theData)
        { 
            await UpdateAsync(_mapper.Map<Base_Department>(theData));
            await SaveCoinConfigAsync(theData.Id, theData.coinConfigList);
        }

        [DataDeleteLog(UserLogType.DepartmentManagement, "Name", "部门名")]
        public async Task DeleteDataAsync(List<string> ids)
        {
            if (await GetIQueryable().AnyAsync(x => ids.Contains(x.ParentId)))
                throw new BusException("禁止删除！请先删除所有子级！");

            await DeleteAsync(ids);
        }

        public async Task<Base_DepartmentDTO> GetTheDataByApiKeyAsync(string apiKey)
        {
            if (apiKey.IsNullOrEmpty())
                return null;
            else
            {
                string cacheKey = $"Cache_APIKEY_{apiKey}";
                if (this._cache.Exists(cacheKey))
                {
                    var cache = this._cache.Get(cacheKey).ChangeType<Base_DepartmentDTO>();
                    return await Task.FromResult(cache);
                }
                else
                { 
                    Expression<Func<Base_Department, Base_DepartmentDTO>> select = (a) => new Base_DepartmentDTO
                    {
                    };
                    select = select.BuildExtendSelectExpre();
                    var q = from a in GetIQueryable().AsExpandable()
                            select @select.Invoke(a);
                    q = q.Where(x => x.ApiKey == apiKey);
                    var result = await q.FirstOrDefaultAsync();
                    if (result != null)
                        this._cache.Add(cacheKey, result);
                    return result;
                }
            }
        }

        public async Task<List<DepartmentCoinConfigDTO>> GetCoinConfigListAsync(string tenantId)
        {
            Expression<Func<TenantCoinConfig, Coin, CoinConfig, DepartmentCoinConfigDTO>> select = (a, b, c) => new DepartmentCoinConfigDTO
            {
                Currency = b.Code,
                CoinConfigName = c.Caption
            };
            select = select.BuildExtendSelectExpre();
            var q = from a in this.Db.GetIQueryable<TenantCoinConfig>().AsExpandable()
                    join b in Db.GetIQueryable<Coin>() on a.CoinID equals b.Id into ab 
                    from b in ab.DefaultIfEmpty()
                    join c in Db.GetIQueryable<CoinConfig>() on a.CoinConfigID equals c.Id into ac
                    from c in ac.DefaultIfEmpty()
                    select @select.Invoke(a, b, c);
            var result = await q.Where(e => e.TenantId == tenantId).ToListAsync();
            return result;
        }
        #endregion

        private async Task SaveCoinConfigAsync(string id, List<TenantCoinConfig> coinConfigList)
        {
            var coinDic = (await this._cacheDataBusiness.GetCoinsAsync()).ToDictionary(e => e.Id, e => e.Code);
            var coinfigIds = coinConfigList.Select(e => e.CoinConfigID);
            var coinConfigDic = await this.Db.GetIQueryable<CoinConfig>().AsExpandable().Where(e => coinfigIds.Contains(e.Id)).ToDictionaryAsync(e => e.Id, e => e.CoinID);

            if (coinConfigList.Any(x => string.IsNullOrEmpty(x.CoinConfigID) || string.IsNullOrEmpty(x.CoinID)))
                throw new Exception($"币种与策略不能为空");

            var values = coinConfigList.Where(e => coinConfigDic[e.CoinConfigID] != e.CoinID).Select(x => coinDic[x.CoinID]).ToList();
            if (values.Count > 0)
                throw new Exception($"设置币种与策略币种不一致:{string.Join(",", values)}"); 

            var repeatValues = coinConfigList.GroupBy(x => x.CoinID)
                .Where(x => !string.IsNullOrEmpty(x.Key) && x.Count() > 1)
                .Select(x => coinDic[x.Key]).ToList();
            if (repeatValues.Count > 0)
                throw new Exception($"以下配置币种值重复:{string.Join(",", repeatValues)}");

            repeatValues = coinConfigList.GroupBy(x => x.CoinConfigID)
                .Where(x => !string.IsNullOrEmpty(x.Key) && x.Count() > 1)
                .Select(x => coinConfigDic[x.Key])
                .ToList();
            if (repeatValues.Count > 0)
                throw new Exception($"以下配置策略项重复:{string.Join(",", repeatValues)}");
            
            coinConfigList.ForEach(aData =>
            {
                aData.Id = IdHelper.GetId();
                aData.CreateTime = DateTime.Now;
                aData.TenantId = id;
            });
            //删除原来
            await this.Db.DeleteSqlAsync<TenantCoinConfig>(e => e.TenantId == id);
            //新增
            await this.Db.InsertAsync<TenantCoinConfig>(coinConfigList); 
        }
    }
}