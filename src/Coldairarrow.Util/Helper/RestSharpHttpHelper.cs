using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coldairarrow.Util.Helper
{
    public class RestSharpHttpHelper
    {
        #region 暴露执行方法
        /// <summary>
        /// 组装Client，Request，并执行Http请求
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="baseUrl">基地址</param>
        /// <param name="relativeUrl">相对地址</param>
        /// <param name="method">请求类型</param>
        /// <param name="lstParam">Get/Put/Delete/Post等参数</param>
        /// <param name="obj">post请求体</param>
        /// <returns></returns>
        public static ResponseMessage<T> RestAction<T>(string baseUrl, string relativeUrl, Method method = Method.GET, List<RestParam> lstParam = null)
        {
            var client = new RestClient(baseUrl);
            return RestMethod<T>(client, InstallRequest(relativeUrl, method, lstParam));
        }
        public static ResponseMessage<T> RestAction<T>(string baseUrl, string relativeUrl, IDictionary<string, object> parameters, Method method = Method.GET)
        {
            var lstParam = parameters.Select(e => new RestParam() { ParamType = EmParType.Param, Key = e.Key, Value = e.Value });
            var client = new RestClient(baseUrl);
            return RestMethod<T>(client, InstallRequest(relativeUrl, method, lstParam));
        }
        /// <summary>
        /// 异步请求无返回值
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="relativeUrl"></param>
        /// <param name="method"></param>
        /// <param name="lstParam"></param>
        /// <param name="obj"></param>
        public static string RestAction(string baseUrl, string relativeUrl, Method method = Method.GET, List<RestParam> lstParam = null)
        {
            var client = new RestClient(baseUrl);
            return RestMethod(client, InstallRequest(relativeUrl, method, lstParam));
        }
        public static string RestAction(string baseUrl, string relativeUrl, IDictionary<string, object> parameters, Method method = Method.GET)
        {
            var lstParam = parameters.Select(e => new RestParam() { ParamType = EmParType.Param, Key = e.Key, Value = e.Value });
            var client = new RestClient(baseUrl);
            return RestMethod(client, InstallRequest(relativeUrl, method, lstParam));
        }
        #endregion

        #region 底层调用，并不暴露方法
        /// <summary>
        /// Http请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="client"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        static ResponseMessage<T> RestMethod<T>(RestClient client, RestRequest request)
        {
            RestResponse restResponse = (RestResponse)client.Execute(request);
            try
            {
                if (restResponse.StatusCode == System.Net.HttpStatusCode.InternalServerError) 
                    throw new Exception(restResponse.Content); 

                var response = new ResponseMessage<T>() { success = true };
                if (!string.IsNullOrWhiteSpace(restResponse.Content))
                    response.data = JsonConvert.DeserializeObject<T>(restResponse.Content);
                return response;
                //return restResponse == null ? new ResponseMessage<T>() :
                //    string.IsNullOrWhiteSpace(restResponse.Content) ? new ResponseMessage<T>() :
                //    JsonConvert.DeserializeObject<ResponseMessage<T>>(restResponse.Content);
            }
            catch (Exception ex)
            {
                return new ResponseMessage<T>() { success = false, error = ex.Message  };
            }
        }

        /// <summary>
        /// 无返回值异步调用
        /// </summary>
        /// <param name="client"></param>
        /// <param name="request"></param>
        static string RestMethod(RestClient client, RestRequest request)
        {
            var response = client.Execute(request);
            return response.Content;
        }

        /// <summary>
        /// 组装Request
        /// </summary>
        /// <param name="relativeUrl"></param>
        /// <param name="method"></param>
        /// <param name="lstParam"></param>
        /// <returns></returns>
        static RestRequest InstallRequest(string relativeUrl, Method method = Method.GET, IEnumerable<RestParam> lstParam = null)
        {
            var request = string.IsNullOrEmpty(relativeUrl) ? new RestRequest(method) : new RestRequest(relativeUrl, method);
            if (lstParam != null)
            {
                foreach (RestParam p in lstParam)
                {
                    switch (p.ParamType)
                    {
                        case EmParType.UrlSegment:
                            request.AddUrlSegment(p.Key, p.Value);
                            break;
                        case EmParType.Param:
                            request.AddParameter(p.Key, p.Value);
                            break;
                        case EmParType.Body:
                            request.AddJsonBody(p.Value);
                            break;
                        default:
                            break;
                    }
                }
            }
            return request;
        }
        #endregion
    }

    public enum EmParType { UrlSegment, Param, Body }

    public class RestParam
    {
        public string Key { set; get; }
        public object Value { set; get; }
        public EmParType ParamType { set; get; }
    }

    public class ResponseMessage<T>
    {
        public bool success { set; get; }

        public T data { set; get; }

        public string error { set; get; }
    }
}

