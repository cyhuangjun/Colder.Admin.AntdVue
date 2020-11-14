using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Coldairarrow.Util
{
    public static partial class Extention
    {
        /// <summary>
        /// Add a parameter
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void AddParameter(this IDictionary<string, object> parameters, string key, string value)
        {
            parameters.Add(key, value);
        }

        /// <summary>
        /// Add a parameter
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void AddParameter(this IDictionary<string, object> parameters, string key, object value)
        {
            parameters.Add(key, value);
        }

        /// <summary>
        /// Add an optional parameter. Not added if value is null
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void AddOptionalParameter(this IDictionary<string, object> parameters, string key, object value)
        {
            if (value != null)
                parameters.Add(key, value);
        }

        /// <summary>
        /// Add an optional parameter. Not added if value is null
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void AddOptionalParameter(this IDictionary<string, string> parameters, string key, string value)
        {
            if (value != null)
                parameters.Add(key, value);
        }

        /// <summary>
        /// Create a query string of the specified parameters
        /// </summary>
        /// <param name="parameters">The parameters to use</param>
        /// <param name="urlEncodeValues">Whether or not the values should be url encoded</param>
        /// <param name="serializationType">How to serialize array parameters</param>
        /// <returns></returns>
        public static string CreateParamString(this IDictionary<string, object> parameters, bool urlEncodeValues, ArrayParametersSerialization serializationType)
        {
            var uriString = "";
            var arraysParameters = parameters.Where(p => p.Value.GetType().IsArray).ToList();
            foreach (var arrayEntry in arraysParameters)
            {
                if (serializationType == ArrayParametersSerialization.Array)
                    uriString += $"{string.Join("&", ((object[])(urlEncodeValues ? WebUtility.UrlEncode(arrayEntry.Value.ToString()) : arrayEntry.Value)).Select(v => $"{arrayEntry.Key}[]={v}"))}&";
                else
                {
                    var array = (Array)arrayEntry.Value;
                    uriString += string.Join("&", array.OfType<object>().Select(a => $"{arrayEntry.Key}={WebUtility.UrlEncode(a.ToString())}"));
                    uriString += "&";
                }
            }

            uriString += $"{string.Join("&", parameters.Where(p => !p.Value.GetType().IsArray).Select(s => $"{s.Key}={(urlEncodeValues ? WebUtility.UrlEncode(s.Value.ToString()) : s.Value)}"))}";
            uriString = uriString.TrimEnd('&');
            return uriString;
        }

        public static string Sign(this IDictionary<string, object> signParameters, string securityKey)
        {
            signParameters = signParameters.Where(kv => kv.Key.ToLower() != "mac").OrderBy(kv => kv.Key).ToDictionary(k => k.Key, k => k.Value);
            signParameters.Add("k", securityKey);
            var paramString = signParameters.CreateParamString(false, ArrayParametersSerialization.MultipleValues);
            var sign = Md5Hex(paramString).ToLower();
            return sign;
        }

        private static string Md5Hex(string data)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] dataHash = md5.ComputeHash(Encoding.UTF8.GetBytes(data));
            StringBuilder sb = new StringBuilder();
            foreach (byte b in dataHash)
            {
                sb.Append(b.ToString("x2").ToLower());
            }
            return sb.ToString();
        }
    }

    /// <summary>
    /// Define how array parameters should be send
    /// </summary>
    public enum ArrayParametersSerialization
    {
        /// <summary>
        /// Send multiple key=value for each entry
        /// </summary>
        MultipleValues,
        /// <summary>
        /// Create an []=value array
        /// </summary>
        Array
    }
}
