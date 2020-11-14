using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Text;

namespace CCPP.Cryptocurrency.Common.ETH
{
    /// <summary>
    /// Ether 常用的工具
    /// </summary>
    public static class EtherUtil
    {
        /// <summary>
        /// decimal ether 转化为 string 0x 十六进制的字符串
        /// </summary>
        /// <param name="ether"></param>
        /// <returns></returns>
        public static string EhterToWeiHex(this decimal ether)
        {
            return ether.EtherToWei().LongDECToHEX();
        }

        /// <summary>
        /// 大值转换为基础单位的十六进制
        /// </summary>
        /// <param name="ether">大的金额</param>
        /// <param name="precision">精度值</param>
        /// <returns></returns>
        public static string BigToUnitHex(this decimal ether, int precision)
        {
            var precisionValue = (decimal)Math.Pow(10, precision);
            return (ether * precisionValue).LongDECToHEX();
        }
        /// <summary>
        /// 大值转换为小值
        /// </summary>
        /// <param name="ether">大的金额</param>
        /// <param name="precision">精度值</param>
        /// <returns></returns>
        public static decimal BigToUnit(this decimal ether, int precision)
        {
            var precisionValue = (decimal)Math.Pow(10, precision);
            return (ether * precisionValue);
        }
        /// <summary>
        /// 小值转换成大值
        /// </summary>
        /// <param name="hex">十六进制数值</param>
        /// <param name="precision">精度值</param>
        /// <returns></returns>
        public static decimal UnitToBig(this string hex, int precision)
        {
            var precisionValue = (decimal)Math.Pow(10, precision);
            return hex.LongHEXToDEC() / precisionValue;
        }
        /// <summary>
        /// 小值转换成大值
        /// </summary>
        /// <param name="value">小值</param>
        /// <param name="precision">精度值</param>
        /// <returns></returns>
        public static decimal UnitToBig(this decimal ether, int precision)
        {
            var precisionValue = (decimal)Math.Pow(10, precision);
            return ether / precisionValue;
        }
        /// <summary>
        /// decimal gwei 转换为 string 0x 十六进制的字符串
        /// </summary>
        /// <param name="gwei"></param>
        /// <returns></returns>
        public static string GweiToWeiHex(this decimal gwei)
        {
            return gwei.GweiToWei().LongDECToHEX();
        }

        /// <summary>
        /// decimal ether 转化为 string 0x 十六进制的字符串，舍弃一部分小数点
        /// </summary>
        /// <param name="ether"></param>
        /// <returns></returns>
        public static string EtherToGweiHex(this decimal ether)
        {
            return ((decimal)ether.EtherToGwei()).LongDECToHEX();
        }

        /// <summary>
        /// 十六进制的 wei 转化为 十进制的 ether
        /// </summary>
        /// <param name="ether"></param>
        /// <returns></returns>
        public static decimal WeiToEtherDec(this string wei)
        {
            return wei.LongHEXToDEC().WeiToEther();
        }

        /// <summary>
        /// 十六进制的gwei 转化为 十进制的 ether
        /// </summary>
        /// <param name="gwei"></param>
        /// <returns></returns>
        public static decimal GweiToEtherDec(this string gwei)
        {
            return gwei.LongHEXToDEC().GweiToEther();
        }

        /// <summary>
        /// 十六进制的gwei 转化为 十进制的 gwei
        /// </summary>
        /// <param name="wei"></param>
        /// <returns></returns>
        public static decimal WeiToGWeiDec(this string wei)
        {
            return wei.LongHEXToDEC().WeiToGwei();
        }

        /// <summary>
        /// Gwei to Wei
        /// </summary>
        /// <param name="gwei">1gei=1,000,000,000</param>
        /// <returns></returns>
        public static decimal GweiToWei(this decimal gwei)
        {
            return (decimal)(gwei * 1000000000);
        }

        /// <summary>
        /// Wei to Gwei
        /// </summary>
        /// <returns></returns>
        public static decimal WeiToGwei(this decimal wei)
        {
            return wei / (decimal)1000000000;
        }

        /// <summary>
        /// Ether To Gwei
        /// </summary>
        /// <param name="ether"></param>
        /// <returns></returns>
        public static decimal EtherToGwei(this decimal ether)
        {
            return ether * 1000000000;
        }

        /// <summary>
        /// Gwei To Ether
        /// </summary>
        /// <param name="gwei"></param>
        /// <returns></returns>
        public static decimal GweiToEther(this decimal gwei)
        {
            return gwei / 1000000000;
        }

        /// <summary>
        /// Ether to Wei
        /// </summary>
        /// <param name="ether"></param>
        /// <returns></returns>
        public static decimal EtherToWei(this decimal ether)
        {
            return (ether * 1000000000000000000);
        }

        /// <summary>
        /// Wei to Ether
        /// </summary>
        /// <param name="wei"></param>
        /// <returns></returns>
        public static decimal WeiToEther(this decimal wei)
        {
            return (wei / (decimal)1000000000000000000);
        }

        /// <summary>
        /// 十六进制 转换 十进制
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static int IntHEXToDECInt(this string hex)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(hex))
                {
                    return 0;
                }
                return Convert.ToInt32(hex, 16);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// 十进制 转换 十六进制
        /// </summary>
        /// <param name="dec"></param>
        /// <returns></returns>
        public static string IntDECToHEX(this int dec)
        {
            return $"0x{Convert.ToString(dec, 16)}";
        }


        /// <summary>
        /// 十六进制 转换 十进制
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static decimal LongHEXToDEC(this string hex)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(hex) || hex == "0x")
                {
                    return 0;
                }
                hex = "0" + hex.Replace("0x", "");
                return (decimal)BigInteger.Parse(hex, NumberStyles.HexNumber);
                //return Convert.ToInt64(hex, 16);
            }
            catch (Exception)
            {
                return 0m;
            }
        }

        /// <summary>
        /// 十六进制 转换 十进制
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static decimal BigIntHEXToDEC(this string hex)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(hex) || hex == "0x")
                {
                    return 0;
                }
                hex = "0" + hex.Replace("0x", "");
                return (decimal)BigInteger.Parse(hex, NumberStyles.HexNumber);
            }
            catch (Exception)
            {
                return 0m;
            }
        }

        /// <summary>
        /// 十进制 转换 十六进制
        /// </summary>
        /// <param name="dec"></param>
        /// <returns></returns>
        public static string LongDECToHEX(this decimal dec)
        {
            BigInteger bigint = (BigInteger)dec;
            var hexData = bigint.ToString("x");
            if (hexData.IndexOf('0') == 0)
            {
                hexData = hexData.Substring(1);
            }
            return "0x" + hexData;
        }
        /// <summary>
        /// 移除十六进开头的0x
        /// </summary>
        /// <param name="dec"></param>
        /// <returns></returns>
        public static string RemoveHex(this string dec)
        {
            if (dec.IndexOf("0x") >= 0)
            {
                dec = dec.Substring(2);
            }
            return dec;
        }
        /// <summary>
        /// 十进制 转换 十六进制
        /// </summary>
        /// <param name="dec"></param>
        /// <returns></returns>
        public static string BitIntDECToHEX(this BigInteger dec)
        {
            return "0x" + dec.ToString("x");
        }

        /// <summary>
        /// 替换 0x0 开头为 0x 
        /// </summary>
        /// <returns></returns>
        public static string ReplaceHex(this string hex)
        {
            return hex.Replace("0x0", "0x");
        }


    }
}
