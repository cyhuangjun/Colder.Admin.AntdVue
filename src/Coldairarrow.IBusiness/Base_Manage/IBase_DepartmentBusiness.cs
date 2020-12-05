using Coldairarrow.Entity;
using Coldairarrow.Entity.Base_Manage;
using Coldairarrow.Util;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Coldairarrow.Business.Base_Manage
{
    public interface IBase_DepartmentBusiness
    {
        Task<List<Base_DepartmentTreeDTO>> GetTreeDataListAsync(DepartmentsTreeInputDTO input);
        Task<Base_Department> GetTheDataAsync(string id);
        Task<List<string>> GetChildrenIdsAsync(string departmentId);
        Task AddDataAsync(Base_Department newData);
        Task UpdateDataAsync(Base_Department theData);
        Task DeleteDataAsync(List<string> ids);
        Task<Base_DepartmentDTO> GetTheDataByApiKeyAsync(string apiKey);

        Task<Base_Department> GetEntityAsync(Expression<Func<Base_Department, bool>> expression);
        List<Base_Department> GetList(Expression<Func<Base_Department, bool>> expression);
        Task<List<Base_Department>> GetListAsync(Expression<Func<Base_Department, bool>> expression);
        List<Base_Department> GetList<TKey>(Expression<Func<Base_Department, bool>> expression, Expression<Func<Base_Department, TKey>> orderByDescending, int pageIndex, int pageSize = 20);
        Task<List<Base_Department>> GetListAsync<TKey>(Expression<Func<Base_Department, bool>> expression, Expression<Func<Base_Department, TKey>> orderByDescending, int pageIndex, int pageSize = 20);
    }

    public class DepartmentsTreeInputDTO
    {
        public string parentId { get; set; }
    }

    public class Base_DepartmentTreeDTO : TreeModel
    {
        public object children { get => Children; }
        public string title { get => Text; }
        public string value { get => Id; }
        public string key { get => Id; }
    }
}