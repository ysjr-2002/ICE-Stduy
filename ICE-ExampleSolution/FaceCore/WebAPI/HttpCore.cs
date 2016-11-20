using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AirPort.Server.WebAPI
{
    public static class HttpCore
    {
        /// <summary>
        ///  把数组所有元素，按照“参数=参数值”的模式用“&”字符拼接成字符串
        /// </summary>
        /// <param name="dicArray"></param>
        /// <returns></returns>
        public static string CreateLinkString(Dictionary<string, string> dicArray)
        {
            if (dicArray == null || dicArray.Count == 0)
                return string.Empty;

            StringBuilder prestr = new StringBuilder();
            foreach (KeyValuePair<string, string> temp in dicArray)
            {
                prestr.Append(temp.Key + "=" + HttpUtility.UrlEncode(temp.Value) + "&");
            }

            //去掉最後一個&字符
            int nLen = prestr.Length;
            prestr.Remove(nLen - 1, 1);

            return prestr.ToString();
        }
    }
}
