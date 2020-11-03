using Coldairarrow.Business.Transaction;
using Coldairarrow.Entity.Transaction;
using Coldairarrow.Util;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coldairarrow.Api.Controllers.Transaction
{
    [Route("/Transaction/[controller]/[action]")]
    public class TransfersController : BaseApiController
    {
        #region DI

        public TransfersController(ITransfersBusiness transfersBus)
        {
            _transfersBus = transfersBus;
        }

        ITransfersBusiness _transfersBus { get; }

        #endregion

        #region 获取

        [HttpPost]
        public async Task<PageResult<Transfers>> GetDataList(PageInput<ConditionDTO> input)
        {
            return await _transfersBus.GetDataListAsync(input);
        }

        [HttpPost]
        public async Task<Transfers> GetTheData(IdInputDTO input)
        {
            return await _transfersBus.GetTheDataAsync(input.id);
        }

        #endregion

        #region 提交

        [HttpPost]
        public async Task SaveData(Transfers data)
        {
            if (data.Id.IsNullOrEmpty())
            {
                InitEntity(data);

                await _transfersBus.AddDataAsync(data);
            }
            else
            {
                await _transfersBus.UpdateDataAsync(data);
            }
        }

        [HttpPost]
        public async Task DeleteData(List<string> ids)
        {
            await _transfersBus.DeleteDataAsync(ids);
        }

        #endregion
    }
}