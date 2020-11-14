using Coldairarrow.Util.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace CCPP.Cryptocurrency.Common.Provider
{
    public abstract class BaseClientServiceProvider
    {
        protected Uri _url;
        private static Action<object> _writeLogCallback;
        /// <summary>
        /// 默认节点
        /// </summary>
        public const string DefaultApiName = "default";
        /// <summary>
        /// 钱包节点
        /// </summary>
        public const string WalletApiName = "wallet";
        /// <summary>
        /// 用于查交易记录
        /// </summary>
        public const string ExplorerApiName = "explorer";
        /// <summary>
        /// 写日志信息
        /// </summary>
        public static Action<object> WriteLog
        {
            get
            {
                return _writeLogCallback;
            }
            set
            {
                _writeLogCallback = value;
            }
        }
        protected ICredentials _credentials;
        protected const int MaxTimeOut = 10000;
        protected Configuration _configuration; 

        public BaseClientServiceProvider(Configuration configuration)
        {
            _configuration = configuration;
            _url = new Uri($"http://{configuration.Host}:{configuration.Port}");
            _credentials = new NetworkCredential(configuration.UserName, configuration.Password);
        } 

        /// <summary>
        /// 回调方法
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="a_sMethod">调用方法</param>
        /// <param name="a_params">调用参数</param>
        /// <returns></returns>
        public virtual ResponseData<T> InvokeMethod<T>(string a_sMethod, string apiName, params object[] a_params)
        {
            if (!string.IsNullOrEmpty(this._configuration.ApiUrl))
            {
                return InvokeEncrypt<T>(a_sMethod, apiName, a_params);
            }
            return InvokeMethodByHost<T>(a_sMethod, apiName, this._configuration.Host, this._configuration.Port, a_params);
        }

        public virtual ResponseData<T> InvokeMethodByHost<T>(string method, string apiName, string host, string port, params object[] parameters)
        {
            return InvokeEncrypt<T>(method, apiName, parameters);
        }

        public virtual ResponseData<T> InvokeEncrypt<T>(string method, string apiName, params object[] parameters)
        {
            return HandlerEncryptByRPC<T>(method, apiName, string.Empty, parameters);
        }
    
        protected ResponseData<T> HandlerEncryptByRPC<T>(string method, string apiName, string jsonrpc, object parameters)
        {
            var requestResult = GetApiRequest(method, apiName, "", parameters, false, jsonrpc);

            var securityKey = EncryptionHelper.Decode(this._configuration.ApiSecurityKey);
            string decodeData = string.Empty;
            if (!string.IsNullOrEmpty(requestResult.Data))
            {
                decodeData = Decode(requestResult.Data, securityKey);
            }
            decodeData = JsonConvert.DeserializeObject<string>(decodeData); 
            if (!requestResult.Status)
            {
                var message = $"请求服务出错,URL:{this._configuration.ApiUrl},错误信息:{requestResult.Code}, Method:{method}, APIName:{apiName}, Parameters:{JsonConvert.SerializeObject(parameters)},返回内容:{decodeData},系统时间:{DateTime.Now},时间戳:{((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds()}";
                Console.WriteLine(message);
                throw new Exception(message);
            }
            JsonRpcResponse<T> ret = null;
            try
            {
                ret = JsonConvert.DeserializeObject<JsonRpcResponse<T>>(decodeData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"反系列化数据出错:{decodeData}");
                throw ex;
            }
            if ((ret != null && ret.Error != null))
            {
                var code = ret != null && ret.Error != null ? ret.Error.Code : 0;
                var msg = ret != null && ret.Error != null ? ret.Error.Message : "Error";

                string parameterString = string.Empty;
                if (parameters != null)
                    parameterString = JsonConvert.SerializeObject(parameters);
                msg += $"请求信息:方法:{method},请求参数:{parameterString},返回内容:{decodeData}";
                throw new Exception(msg);
            }
            var result = new ResponseData<T>
            {
                Result = ret.Result,
                Error = ret.Error?.Message
            };
            return result;
        }
     
        protected ApiResponse GetApiRequest(string method, string apiName, string httpMethod, object parameters, bool isHttp, string jsonRPC)
        {
            var securityKey = EncryptionHelper.Decode(this._configuration.ApiSecurityKey);
            var seed = GetSeed();
            ApiRPCInfo rpcInfo = null;
            if (!string.IsNullOrEmpty(jsonRPC))
            {
                rpcInfo = new ApiRPCVersionInfo
                {
                    id = 1,
                    jsonrpc = jsonRPC,
                    @params = parameters
                };
            }
            else
            {
                rpcInfo = new ApiRPCInfo
                {
                    id = 1,
                    @params = parameters

                };
            }
            if (!string.IsNullOrEmpty(httpMethod))
            {
                httpMethod = httpMethod.ToUpper();
            }
            var requestData = new ApiRequestData
            {
                Api = apiName,
                HttpMethod = httpMethod
            };
            if (isHttp)
            {
                requestData.Path = method;
                if (parameters != null)
                {
                    requestData.Body = JsonConvert.SerializeObject(parameters);
                }
            }
            else
            {
                rpcInfo.method = method;
                requestData.Body = JsonConvert.SerializeObject(rpcInfo);
            }

            var requestJsonData = JsonConvert.SerializeObject(requestData);
            var securityData = Encode(requestJsonData, securityKey);
            var timestamp = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();
            var shaData = Encoding.UTF8.GetBytes(seed + timestamp + securityData);
            string sha = string.Empty;
            using (var sha1 = SHA1.Create())
            {
                var shaBuffer = sha1.ComputeHash(shaData);
                sha = Convert.ToBase64String(shaBuffer);
            }

            var apiRequest = new ApiRequest
            {
                Seed = seed,
                Data = securityData,
                Date = timestamp,
                Hash = sha
            };
            var host = this._configuration.ApiUrl;
            var requestResult = RestSharpHttpHelper.RestAction<ApiResponse>(host, "/RPC", RestSharp.Method.POST, new List<RestParam>() 
            { 
                new RestParam() {  ParamType = EmParType.Body, Value = apiRequest } 
            }); 
            if(requestResult.success)
                return requestResult.data;
            throw new Exception(requestResult.error);
        }
        protected string GetSeed()
        {
            var host = this._configuration.ApiUrl;
            var requestResult = RestSharpHttpHelper.RestAction<ApiResponse>(host, "/Seeds", RestSharp.Method.POST);
            if (!requestResult.success)
                throw new Exception(requestResult.error);
            if (!requestResult.data.Status)
            {
                throw new Exception($"请求失败,失败原因:{requestResult.data.Code}");
            }
            return requestResult.data.Data;
        }

        protected string Encode(string text, string securityKey)
        {
            return EncryptionHelper.Encode(text, securityKey);
        }

        protected string Decode(string cipherText, string keyString)
        {
            return EncryptionHelper.Decode(cipherText, keyString);
        }
    }
}
