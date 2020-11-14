using System;
using System.Collections.Generic;
using System.Text;

namespace CCPP.Cryptocurrency.Common.Utils
{
    public static class Extensions
    {
        /// <summary> 
        /// 根据GUID获取16位的唯一字符串 
        /// </summary> 
        /// <param name=\"guid\"></param> 
        /// <returns></returns> 
        public static string GuidTo22String(this Guid guid, int length = 22)
        {
            var data = guid.ToString().Replace("-", "");
            string base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(data));

            string encoded = base64.Replace("/", "").Replace("+", "");

            return encoded.Substring(0, length);
        }

        public static string GuidTo64String(this Guid guid)
        {
            var data = guid.ToString().Replace("-", "");
            var newData = Guid.NewGuid().ToString().Replace("-", "");
            return data + newData;
        }

        /// <summary>
        /// 截断指定小数
        /// </summary>
        /// <param name="data"></param>
        /// <param name="scale">小数位长度</param>
        /// <returns></returns>
        public static decimal DataTruncated(this decimal data, int scale)
        {
            var strData = data.ToString(scale);
            return decimal.Parse(strData);
        }
        /// <summary>
        /// decimal保留指定位数小数
        /// </summary>
        /// <param name="num">原始数量</param>
        /// <param name="scale">保留小数位数</param>
        /// <returns>截取指定小数位数后的数量字符串</returns>
        public static string ToString(this decimal num, int scale)
        {
            string numToString = num.ToString();

            int index = numToString.IndexOf(".");
            int length = numToString.Length;

            if (index != -1)
            {
                if (scale == 0)
                {
                    return numToString.Substring(0, index);
                }
                return string.Format("{0}.{1}",
                    numToString.Substring(0, index),
                    numToString.Substring(index + 1, Math.Min(length - index - 1, scale)));
            }
            else
            {
                return num.ToString($"f{scale}");
            }
        }
        public static string ConvertToHex(this byte[] data)
        {
            return BitConverter.ToString(data, 0).Replace("-", "");
        }
        public static string ConvertStringToHex(this string data)
        {
            StringBuilder sbHex = new StringBuilder();
            foreach (char chr in data)
            {
                sbHex.Append(String.Format("{0:X2}", Convert.ToInt32(chr)));
            }
            return sbHex.ToString();
        }
    }
}
