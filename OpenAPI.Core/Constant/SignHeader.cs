using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenAPI.Core.Constant
{
    /// <summary>
    /// Api签名Hearder中添加的内容
    /// </summary>
    public class SignHeader
    {
       
        /// <summary>
        /// 签名结果
        /// </summary>
        public const string X_CA_SIGNATURE = Constants.SIGN_HEADER_PREFIX + "Signature";
        /// <summary>
        /// 时间戳 
        /// 设置时注意对应值的格式"yyyyMMddHHmmss"
        /// </summary>
        public const string X_CA_TIME = Constants.SIGN_HEADER_PREFIX + "Time";
        /// <summary>
        /// 指定对应APPKey
        /// 用于api端通过对应的appKey来找到对应的SecretKey来进行签名验证
        /// </summary>
        public const string X_CA_APP_KEY = Constants.SIGN_HEADER_PREFIX + "App_Key";
    }
}
