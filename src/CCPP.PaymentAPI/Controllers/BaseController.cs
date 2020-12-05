using Coldairarrow.Business.Base_Manage;
using Coldairarrow.Entity;
using Coldairarrow.Util;
using Microsoft.AspNetCore.Mvc;

namespace CCPP.PaymentAPI.Controllers
{
    /// <summary>
    /// 基控制器
    /// </summary>
    public class BaseController : ControllerBase
    {
        private readonly IBase_DepartmentBusiness _base_DepartmentBusiness;

        public BaseController(IBase_DepartmentBusiness base_DepartmentBusiness)
        {
            this._base_DepartmentBusiness = base_DepartmentBusiness;
        }   

        protected Base_DepartmentDTO CurrentTenant
        {
            get
            {
                string apiKey = this.HttpContext.Request.Headers[GlobalData.HTTPHEADAPIKEY];
                if (apiKey.IsNullOrEmpty())
                    return null;
                var task = this._base_DepartmentBusiness.GetTheDataByApiKeyAsync(apiKey);
                task.Wait();
                return task.Result;
            }
        }

        protected string Sign(object parameter, string securityKey)
        {
            if (parameter == null) return string.Empty;
            var signParameters = parameter.ToDictionary();
            return signParameters.Sign(securityKey);
        }
    }
}
