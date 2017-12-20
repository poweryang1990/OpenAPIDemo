using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using OpenAPI.Core.Constant;

namespace OpenAPI.Core
{
    public abstract class BaseApiSign
    {
        /// <summary>
        /// 签名   
        /// </summary>
        /// <param name="path">请求路径</param>
        /// <param name="secretKey">加密的Key</param>
        /// <param name="headers">header内容</param>
        /// <param name="queryString">QueryString参数</param>
        /// <param name="body">Post请求 Body参数</param>
        /// <param name="signHeaderPrefixList">需要签名Header内容的前缀</param>
        /// <returns></returns>
        public string Sign(string path, string secretKey, NameValueCollection headers, NameValueCollection queryString,NameValueCollection body, List<string> signHeaderPrefixList=null)
        {
            var plain = MakeSignPlain(path, headers, queryString, body);
            return Hash(secretKey, plain);
        }

        public string MakeSignPlain(string path, NameValueCollection headers, NameValueCollection queryString, NameValueCollection body, List<string> signHeaderPrefixList=null)
        {
            StringBuilder sb = new StringBuilder();

            //header参与签名
            StringBuilder sbHeader = new StringBuilder();
            IDictionary<string, string> sortHeaders = new SortedDictionary<string, string>(StringComparer.Ordinal);
            
            if (headers != null && headers.Count > 0)
            {
                foreach (var key in headers.AllKeys)
                {
                    if (key.Length > 0)
                    {
                        sortHeaders.Add(key, headers[key]);
                    }
                }
            }
            foreach (var param in sortHeaders)
            {
                if (IsHeaderToSign(param.Key, signHeaderPrefixList))
                {
                    if (sbHeader.Length > 0)
                    {
                        sbHeader.Append(Constants.SPE1);
                    }
                    sbHeader.Append(param.Key).Append(Constants.SPE2);
                    if (param.Value != null)
                    {
                        sbHeader.Append(param.Value);
                    }
                }
            }
            if (sbHeader.Length > 0)
            {
                sb.Append(sbHeader).Append(Constants.LF);
            }

            //path参与签名
            if (path != null)
            {
                sb.Append(path);
            }
            StringBuilder sbParam = new StringBuilder();
            IDictionary<string, string> sortParams = new SortedDictionary<string, string>(StringComparer.Ordinal);
            //queryString参与签名
            if (queryString!=null&&queryString.Count>0)
            {
                foreach (var key in queryString.AllKeys)
                {
                    if (key.Length>0)
                    {
                        sortParams.Add(key, queryString[key]);
                    }
                }
            }
            //body参与签名
            if (body != null && body.Count > 0)
            {
                foreach (var key in body.AllKeys)
                {
                    if (key.Length > 0)
                    {
                        sortParams.Add(key, body[key]);
                    }
                }
            }

            //参数字符拼接           
            foreach (var param in sortParams)
            {
                if ( param.Key.Length>0)
                {
                    if ( sbParam.Length>0)
                    {
                        sbParam.Append("&");
                    }
                    sbParam.Append(param.Key);
                    if (!string.IsNullOrEmpty(param.Value))
                    {
                        sbParam.Append("=").Append(param.Value);
                    }
                }
            }
            if (sbParam.Length>0)
            {
                sb.Append("?").Append(sbParam);
            }      
            
            return sb.ToString();
        }

        /**
        * Http头是否参与签名
        * return
        */
        private static bool IsHeaderToSign(string headerName, List<string> signHeaderPrefixList)
        {
            if (string.IsNullOrEmpty(headerName))
            {
                return false;
            }
            //以X_Ca_开头的默认参与签名 但签名计算
            if (headerName.StartsWith(Constants.SIGN_HEADER_PREFIX)&&!headerName.Equals(SignHeader.X_CA_SIGNATURE,StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }

            if (signHeaderPrefixList != null)
            {
                foreach (var signHeaderPrefix in signHeaderPrefixList)
                {
                    if (headerName.StartsWith(signHeaderPrefix))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        /// <summary>
        /// 签名算法
        /// Hash散列算法
        /// </summary>
        /// <param name="secretKey"></param>
        /// <param name="plain"></param>
        /// <returns></returns>
        public abstract string Hash(string secretKey, string plain);

        /// <summary>
        /// 验证请求
        /// </summary>
        /// <param name="requestSign">Header中携带过来的签名结果</param>
        /// <param name="signPlain">实际请求需要签名的内容</param>
        /// <param name="time">Header中携带过来的时间</param>
        /// <param name="secretKey"></param>
        /// <returns></returns>
        public bool Valid(string requestSign, string signPlain, string time, string secretKey)
        {
            if (string.IsNullOrEmpty(time) || string.IsNullOrEmpty(requestSign) || string.IsNullOrEmpty(signPlain))
            {
                return false;
            }
            //is in range
            var now = DateTime.Now;
            long requestTime = 0;
            if (long.TryParse(time, out requestTime))
            {
                var max = now.AddMinutes(5).ToString("yyyyMMddHHmmss");
                var min = now.AddMinutes(-5).ToString("yyyyMMddHHmmss");
                if (requestTime>long.Parse(max)|| requestTime<long.Parse(min))
                {
                    return false;
                }

            }
            else
            {
                return false;
            }
            var sign = Hash(secretKey, signPlain);

            return requestSign.Equals(sign, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}