using CacheManager.Core;
using Coldairarrow.Entity;
using Coldairarrow.Entity.Base_Manage;
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
        ICacheManager<object> _cache;
        public Base_DepartmentBusiness(IDbAccessor db,
                                        ICacheManager<object> cache)
            : base(db)
        {
            _cache = cache;
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
                    Value = x.Id
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

        public async Task<Base_Department> GetTheDataAsync(string id)
        {
            return await GetEntityAsync(id);
        }

        [DataRepeatValidate(new string[] { "Name" }, new string[] { "部门名" })]
        [DataAddLog(UserLogType.DepartmentManagement, "Name", "部门名")]
        public async Task AddDataAsync(Base_Department newData)
        {
            await InsertAsync(newData);
        }

        [DataRepeatValidate(new string[] { "Name" }, new string[] { "部门名" })]
        [DataEditLog(UserLogType.DepartmentManagement, "Name", "部门名")]
        public async Task UpdateDataAsync(Base_Department theData)
        {
            await UpdateAsync(theData);
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
                    var theData = await this.GetEntityAsync(e => e.ApiKey == apiKey);
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
        #endregion
    }
}