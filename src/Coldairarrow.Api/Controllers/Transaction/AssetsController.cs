using Coldairarrow.Business.Transaction;
using Coldairarrow.Entity.Transaction;
using Coldairarrow.Util;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Coldairarrow.IBusiness;
using Coldairarrow.Business.Base_Manage;
using System.Linq;

namespace Coldairarrow.Api.Controllers.Transaction
{
    [Route("/Transaction/[controller]/[action]")]
    public class AssetsController : BaseApiController
    {
        #region DI
        IAssetsBusiness _assetsBus { get; }
        IBase_DepartmentBusiness _base_DepartmentBusiness;
        public AssetsController(IAssetsBusiness assetsBus,
                                IBase_DepartmentBusiness base_DepartmentBusiness)
        {
            _assetsBus = assetsBus;
            _base_DepartmentBusiness = base_DepartmentBusiness;
        } 
        #endregion

        #region 获取

        [HttpPost]
        public async Task<PageResult<AssetsOutDTO>> GetDataList(PageInput<AssetsInputDTO> input)
        {
            var op = HttpContext.RequestServices.GetService<IOperator>();
            if (!op.IsAdmin())
                input.Search.TenantId = op.TenantId;
            if (!string.IsNullOrEmpty(input.Search.TenantId))
                await _assetsBus.InitTenantAssets(input.Search.TenantId);
            return await _assetsBus.GetDataListAsync(input);
        }

        //[HttpPost]
        //public async Task<Assets> GetTheData(IdInputDTO input)
        //{
        //    return await _assetsBus.GetTheDataAsync(input.id);
        //} 
        #endregion
    }
}