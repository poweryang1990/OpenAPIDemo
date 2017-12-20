using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenAPI.Core.Constant;

namespace OpenAPI.Core.Filter
{
    /// <summary>
    /// 内部api签名验证过滤器
    /// </summary>
    public class InnerSignAuthenticationAttribute: BaseSignAuthenticationAttribute
    {
        public override BaseApiSign GetSignner()
        {
           return new HMACSHA256Sign();
        }

        public override Dictionary<string, string> SecretDic()
        {
            var dic = new Dictionary<string, string>
            {
                {Constants.INNER_DEFAULT_APPKEY, Constants.INNER_DEFAULT_APPSECRET}
            };
            return dic;
        }

        public override List<string> SignHeaderPrefixList()
        {
            return null;
        }
    }
}
