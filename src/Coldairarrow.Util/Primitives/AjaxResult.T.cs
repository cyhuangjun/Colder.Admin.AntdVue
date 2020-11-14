namespace Coldairarrow.Util
{
    /// <summary>
    /// Ajax请求结果
    /// </summary>
    public class AjaxResult<T> : AjaxResult
    {
        /// <summary>
        /// 返回数据
        /// </summary>
        public T Data { get; set; }
        /// <summary>
        /// 签名
        /// </summary>
        public string Mac { set; get; }
    }
}
