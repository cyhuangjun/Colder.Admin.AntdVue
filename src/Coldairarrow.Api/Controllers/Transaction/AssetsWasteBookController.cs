using Coldairarrow.Business.Transaction;
using Coldairarrow.Entity.Transaction;
using Coldairarrow.IBusiness;
using Coldairarrow.Util;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Coldairarrow.Api.Controllers.Transaction
{
    [Route("/Transaction/[controller]/[action]")]
    public class AssetsWasteBookController : BaseApiController
    {
        #region DI

        public AssetsWasteBookController(IAssetsWasteBookBusiness assetsWasteBookBus)
        {
            _assetsWasteBookBus = assetsWasteBookBus;
        }

        IAssetsWasteBookBusiness _assetsWasteBookBus { get; }

        #endregion

        #region 获取

        [HttpPost]
        public async Task<PageResult<AssetsWasteBookOutDTO>> GetDataList(PageInput<AssetsWasteBookInputDTO> input)
        {
            var op = HttpContext.RequestServices.GetService<IOperator>();
            if (!op.IsAdmin())
                input.Search.TenantId = op.TenantId;
            return await _assetsWasteBookBus.GetDataListAsync(input);
        }

        //[HttpPost]
        //public async Task<AssetsWasteBook> GetTheData(IdInputDTO input)
        //{
        //    return await _assetsWasteBookBus.GetTheDataAsync(input.id);
        //}

        #endregion 
    }
}