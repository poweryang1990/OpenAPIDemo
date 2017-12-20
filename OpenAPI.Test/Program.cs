using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using OpenAPI.Core;
using OpenAPI.Core.Constant;

namespace OpenAPI.Test
{
    class Program
    {
        private const String host = "http://localhost:2729";

        static void Main(string[] args)
        {

            while (true)
            {
               Console.WriteLine("输入 数字进行测试 1::Get 2:Post");
                var result = Console.ReadLine();
                switch (result)
                {
                    case "1":
                        GetMethodDemo();
                        break;
                    case "2":
                        PostMethodDemo();
                        break;
                    default:
                        GetMethodDemo();
                        break;
                }
            }
        }

        private static void GetMethodDemo()
        {
            var path = "/api/Values/Show";

            var queryString = new NameValueCollection()
            {
                {"name", "PowerYang"},
                {"age", "20"}
            };
            var headers = new NameValueCollection()
            {
                {HttpHeader.HTTP_HEADER_CONTENT_TYPE,ContentType.CONTENT_TYPE_JSON},
            };

            var httpUtilForSign = new HttpUtilForSign(new HMACSHA256Sign());
            var result = httpUtilForSign.HttpGet(host, path, Constants.INNER_DEFAULT_APPKEY,Constants.INNER_DEFAULT_APPSECRET, 2*60, headers, queryString);
            Console.WriteLine(result);
        }

        private static void PostMethodDemo()
        {
            var path = "/api/Values/Add";

            var body = new NameValueCollection()
            {
                {"name", "PowerYang"},
                {"age", "20"}
            };
            var headers = new NameValueCollection()
            {
                {HttpHeader.HTTP_HEADER_CONTENT_TYPE,ContentType.CONTENT_TYPE_FORM}
            };

            var httpUtilForSign = new HttpUtilForSign(new HMACSHA256Sign());
            var result = httpUtilForSign.HttpPost(host, path, Constants.INNER_DEFAULT_APPKEY,Constants.INNER_DEFAULT_APPSECRET, 2*60, headers, null, body);
            Console.WriteLine(result);
        }
    }
}
