using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenAPI.Core.Constant
{
    public class Constants
    {
        /// <summary>
        /// 签名专用Header前缀
        /// </summary>
        public const string SIGN_HEADER_PREFIX = "X_Ca_";
        /// <summary>
        /// 换行符
        /// </summary>
        public const string LF = "\n";
        /// <summary>
        /// 逗号分隔符
        /// </summary>
        public const string SPE1 = ",";
        /// <summary>
        /// 冒号分隔符
        /// </summary>
        public const string SPE2 = ":";
        /// <summary>
        /// 内部默认的AppKey
        /// </summary>
        public const string INNER_DEFAULT_APPKEY = "uoko-star";
        /// <summary>
        /// 内部默认APP的SecretKey
        /// </summary>

        public const string INNER_DEFAULT_APPSECRET = "6HuNqq8b9hwzq8VnGIbYzQ8N569RZCXe";
    }
}
