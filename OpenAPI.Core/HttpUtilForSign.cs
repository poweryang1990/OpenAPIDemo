using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using OpenAPI.Core.Constant;

namespace OpenAPI.Core
{
    /// <summary>
    /// 调用签名接口
    /// </summary>
    public class HttpUtilForSign
    {
        private readonly BaseApiSign _signner;
        public HttpUtilForSign(BaseApiSign signner)
        {
            _signner = signner;
        }
        /// <summary>
        /// Post方法
        /// </summary>
        /// <param name="host">API的地址 如 http://domain</param>
        /// <param name="path">API url path</param>
        /// <param name="appKey"></param>
        /// <param name="appSecret"></param>
        /// <param name="timeout">超时时间 单位-秒</param>
        /// <param name="headers"></param>
        /// <param name="queryString"></param>
        /// <param name="body"></param>
        /// <param name="signHeaderPrefixList">需要签名的Header前缀</param>
        /// <returns></returns>
        public string HttpPost(string host, string path, string appKey, string appSecret, int timeout, NameValueCollection headers, NameValueCollection queryString, NameValueCollection body, List<string> signHeaderPrefixList = null)
        {
            return DoHttp(host, path, HttpMethod.POST, appKey, appSecret, timeout, headers, queryString, body, signHeaderPrefixList);
        }
        /// <summary>
        /// Put方法
        /// </summary>
        /// <param name="host">API的地址 如 http://domain</param>
        /// <param name="path">API url path</param>
        /// <param name="appKey"></param>
        /// <param name="appSecret"></param>
        /// <param name="timeout">超时时间 单位-秒</param>
        /// <param name="headers"></param>
        /// <param name="queryString"></param>
        /// <param name="body"></param>
        /// <param name="signHeaderPrefixList">需要签名的Header前缀</param>
        /// <returns></returns>
        public string HttpPut(string host, string path, string appKey, string appSecret, int timeout, NameValueCollection headers, NameValueCollection queryString, NameValueCollection body, List<string> signHeaderPrefixList = null)
        {
            return DoHttp(host, path, HttpMethod.PUT, appKey, appSecret, timeout, headers, queryString, body, signHeaderPrefixList);
        }
        /// <summary>
        /// Get方法
        /// </summary>
        /// <param name="host">API的地址 如 http://domain</param>
        /// <param name="path">API url path</param>
        /// <param name="appKey"></param>
        /// <param name="appSecret"></param>
        /// <param name="timeout">超时时间 单位-秒</param>
        /// <param name="headers"></param>
        /// <param name="queryString"></param>
        /// <param name="signHeaderPrefixList">需要签名的Header前缀</param>
        /// <returns></returns>
        public string HttpGet(string host, string path, string appKey, string appSecret, int timeout, NameValueCollection headers, NameValueCollection queryString,List<string> signHeaderPrefixList = null)
        {
            return DoHttp(host, path, HttpMethod.GET, appKey, appSecret, timeout, headers, queryString, null, signHeaderPrefixList);
        }
        /// <summary>
        /// Head方法
        /// </summary>
        /// <param name="host">API的地址 如 http://domain</param>
        /// <param name="path">API url path</param>
        /// <param name="appKey"></param>
        /// <param name="appSecret"></param>
        /// <param name="timeout">超时时间 单位-秒</param>
        /// <param name="headers"></param>
        /// <param name="queryString"></param>
        /// <param name="signHeaderPrefixList">需要签名的Header前缀</param>
        /// <returns></returns>
        public string HttpHead(string host, string path, string appKey, string appSecret, int timeout, NameValueCollection headers, NameValueCollection queryString,List<string> signHeaderPrefixList = null)
        {
            return DoHttp(host, path, HttpMethod.HEAD, appKey, appSecret, timeout, headers, queryString, null, signHeaderPrefixList);
        }
        /// <summary>
        /// Delate方法
        /// </summary>
        /// <param name="host">API的地址 如 http://domain</param>
        /// <param name="path">API url path</param>
        /// <param name="appKey"></param>
        /// <param name="appSecret"></param>
        /// <param name="timeout">超时时间 单位-秒</param>
        /// <param name="headers"></param>
        /// <param name="queryString"></param>
        /// <param name="signHeaderPrefixList">需要签名的Header前缀</param>
        /// <returns></returns>
        public string HttpDelete(string host, string path, string appKey, string appSecret, int timeout, NameValueCollection headers, NameValueCollection queryString, List<string> signHeaderPrefixList = null)
        {
            return DoHttp(host, path, HttpMethod.DELETE, appKey, appSecret, timeout, headers, queryString, null, signHeaderPrefixList);
        }



        private  string DoHttp(string host, string path, string method, string appKey, string appSecret, int timeout, NameValueCollection headers, NameValueCollection queryString, NameValueCollection body, List<string> signHeaderPrefixList = null)
        {
            headers = InitialBasicHeader(path, appKey, appSecret, headers, queryString, body, signHeaderPrefixList);
            HttpWebRequest httpRequest = InitHttpRequest(host, path, method, timeout, headers, queryString);

            if (body != null && body.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var key in body.AllKeys)
                {
                    if (sb.Length > 0)
                    {
                        sb.Append("&");
                    }
                    if (key?.Length > 0)
                    {
                        sb.Append(key).Append("=");
                        if (body[key] != null)
                        {
                            sb.Append(HttpUtility.UrlEncode(body[key], Encoding.UTF8));
                        }
                    }
                }
                if (sb.Length>0)
                {
                    //只要Body中有内容这指定ContentType
                   httpRequest.ContentType = ContentType.CONTENT_TYPE_FORM;
                   byte[] data = Encoding.UTF8.GetBytes(sb.ToString());
                    using (Stream stream = httpRequest.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    } 
                }     
            }

            using (var responseStream = httpRequest.GetResponse().GetResponseStream())
            {
                using (var streamReader = new StreamReader(responseStream, Encoding.UTF8))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }



        private  HttpWebRequest InitHttpRequest(string host, string path, string method, int timeout, NameValueCollection headers, NameValueCollection queryString)
        {
            HttpWebRequest httpRequest = null;
            string url = host;
            if (null != path)
            {
                url = url + path;
            }

            if (queryString!=null&&queryString.Count>0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var key in queryString.AllKeys)
                {
                    if (sb.Length>0)
                    {
                        sb.Append("&");
                    }
                   
                    if (key?.Length>0)
                    {
                        sb.Append(key).Append("=");
                        if (queryString[key]!=null)
                        {
                            sb.Append(HttpUtility.UrlEncode(queryString[key], Encoding.UTF8));
                        }
                    }
                }
                if (0 < sb.Length)
                {
                    url = url + "?" + sb.ToString();
                }
            }

            if (host.Contains("https://"))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                httpRequest = (HttpWebRequest)WebRequest.CreateDefault(new Uri(url));
            }
            else
            {
                httpRequest = (HttpWebRequest)WebRequest.Create(url);
            }
            httpRequest.ServicePoint.Expect100Continue = false;
            httpRequest.Method = method;
            httpRequest.KeepAlive = true;
            httpRequest.Timeout = timeout*1000;

            if (headers.AllKeys.Contains("Accept"))
            {
                httpRequest.Accept =Pop(headers, "Accept");
            }
            if (headers.AllKeys.Contains("Date"))
            {
                httpRequest.Date = Convert.ToDateTime(Pop(headers, "Date"));
            }
            if (headers.AllKeys.Contains(HttpHeader.HTTP_HEADER_CONTENT_TYPE))
            {
                httpRequest.ContentType =Pop(headers, HttpHeader.HTTP_HEADER_CONTENT_TYPE);
            }
           
            foreach (var headerKey in headers.AllKeys)
            {
                httpRequest.Headers.Add(headerKey, headers[headerKey]);
            }
            return httpRequest;
        }


        private  NameValueCollection InitialBasicHeader(string path, string appKey, string appSecret, NameValueCollection headers, NameValueCollection queryString, NameValueCollection body, List<string> signHeaderPrefixList = null)
        {
            if (headers == null)
            {
                headers =new NameValueCollection();
            }
            headers.Add(SignHeader.X_CA_APP_KEY, appKey);
            headers.Add(SignHeader.X_CA_TIME,DateTime.Now.ToString("yyyyMMddHHmmss"));
            headers.Add(SignHeader.X_CA_SIGNATURE, _signner.Sign(path, appSecret, headers, queryString,body, signHeaderPrefixList));

            return headers;
        }


        public  bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }

        private  string Pop(NameValueCollection collection, string key)
        {
            string value = null;
            if (collection.AllKeys.Contains(key))
            {
                value = collection[key];
                collection.Remove(key);
            }
            return value;
        }
    }
}
