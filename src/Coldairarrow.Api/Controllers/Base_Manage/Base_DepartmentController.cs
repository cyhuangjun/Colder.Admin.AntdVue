using Coldairarrow.Business.Base_Manage;
using Coldairarrow.Entity.Base_Manage;
using Coldairarrow.IBusiness;
using Coldairarrow.Util;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace Coldairarrow.Api.Controllers.Base_Manage
{
    /// <summary>
    /// 部门
    /// </summary>
    [Route("/Base_Manage/[controller]/[action]")]
    [OpenApiTag("部门")]
    public class Base_DepartmentController : BaseApiController
    {
        #region DI

        public Base_DepartmentController(IBase_DepartmentBusiness departmentBus)
        {
            _departmentBus = departmentBus;
        }

        IBase_DepartmentBusiness _departmentBus { get; }

        #endregion

        #region 获取

        [HttpPost]
        public async Task<Base_Department> GetTheData(IdInputDTO input)
        {
            return await _departmentBus.GetTheDataAsync(input.id) ?? new Base_Department();
        }

        [HttpPost]
        public async Task<List<Base_DepartmentTreeDTO>> GetTreeDataList(DepartmentsTreeInputDTO input)
        {
            return await _departmentBus.GetTreeDataListAsync(input);
        }

        [HttpPost]
        public async Task<List<DepartmentCoinConfigDTO>> GetCoinConfigList(IdInputDTO input)
        { 
            return await _departmentBus.GetCoinConfigListAsync(input.id);
        }

        [HttpPost]
        public async Task<List<SelectOption>> GetTenantList()
        {
            var op = HttpContext.RequestServices.GetService<IOperator>();
            var tenants = await this._departmentBus.GetListAsync(e => 1 == 1);
            if (!op.IsAdmin())
                tenants = tenants.Where(e => e.Id == op.TenantId).ToList();
            List<SelectOption> list = new List<SelectOption>();
            foreach (var aValue in tenants)
            {
                list.Add(new SelectOption
                {
                    value = aValue.Id,
                    text = aValue.Name
                });
            }
            return list;
        }
        #endregion

        #region 提交
        [ApiPermission("Base_Department.Add")]
        [ApiPermission("Base_Department.Edit")]
        [HttpPost]
        public async Task SaveData(DepartmentEditInputDTO theData)
        {
            if (theData.Id.IsNullOrEmpty())
            {
                InitEntity(theData);

                await _departmentBus.AddDataAsync(theData);
            }
            else
            {
                await _departmentBus.UpdateDataAsync(theData);
            }
        }
         
        //[ApiPermission("Base_Department.Delete")]
        //[HttpPost]
        //public async Task DeleteData(List<string> ids)
        //{
        //    await _departmentBus.DeleteDataAsync(ids);
        //}

        #endregion
    }
}