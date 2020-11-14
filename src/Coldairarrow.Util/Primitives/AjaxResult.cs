using Microsoft.EntityFrameworkCore.Storage;

namespace Coldairarrow.Util
{
    /// <summary>
    /// Ajax请求结果
    /// </summary>
    public class AjaxResult
    {
        bool success = true;
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success 
        { 
            get 
            {
                if (this.ErrorCode > 0)
                    return false;
                else
                    return success;
            } 
            set 
            { 
                success = value; 
            } 
        }

        /// <summary>
        /// 错误代码
        /// </summary>
        public int ErrorCode { get; set; }

        /// <summary>
        /// 返回消息
        /// </summary>
        public string Msg { get; set; }
    }
}
