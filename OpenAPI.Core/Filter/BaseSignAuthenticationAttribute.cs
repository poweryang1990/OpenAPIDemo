using OpenAPI.Core.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace OpenAPI.Core.Filter
{
    public abstract class BaseSignAuthenticationAttribute : ActionFilterAttribute
    {
        private  Dictionary<string, string> secretDic => SecretDic();
        private List<string> signHeaderPrefixList => SignHeaderPrefixList();
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var request = ((HttpContextWrapper)actionContext.Request.Properties["MS_HttpContext"]).Request;
            var path = request.Path;
            var queryString = request.QueryString;
            var body = request.Form;
            var hearders = request.Headers;

            var time = hearders[SignHeader.X_CA_TIME];
            var appKey = hearders[SignHeader.X_CA_APP_KEY];
            var requestSign = hearders[SignHeader.X_CA_SIGNATURE];
            var appSecret= Constants.INNER_DEFAULT_APPSECRET;

            if (!string.IsNullOrEmpty(appKey))
            {
                appSecret = secretDic[appKey];
            }

            var signner = GetSignner();

            var signPlain = signner.MakeSignPlain(path, hearders, queryString, body, signHeaderPrefixList);

            if (signner.Valid(requestSign, signPlain, time, appSecret))
            {
                //验证通过,执行基类方法  
                base.OnActionExecuting(actionContext);
                return;
            }
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized,"签名错误");

        }
        /// <summary>
        /// 获取签名方法
        /// </summary>
        /// <returns></returns>
        public abstract BaseApiSign GetSignner();
        /// <summary>
        /// 签名AppKey与AppSecret的键值对集合
        /// 一般从内存和缓存中取
        /// </summary>
        /// <returns></returns>
        public  abstract Dictionary<string, string> SecretDic();
        /// <summary>
        /// 指定需要签名的header前缀
        /// </summary>
        /// <returns></returns>
        public abstract List<string> SignHeaderPrefixList();
    }
}
