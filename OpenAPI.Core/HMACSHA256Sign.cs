using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OpenAPI.Core
{
    /// <summary>
    /// HMAC SHA256签名
    /// </summary>
    public class HMACSHA256Sign : BaseApiSign
    {
        public override string Hash(string secretKey, string plain)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var plainBytes = Encoding.UTF8.GetBytes(plain);

            using (var hmacsha256 = new HMACSHA256(keyBytes))
            {
                var sb = new StringBuilder();
                var hashValue = hmacsha256.ComputeHash(plainBytes);
                foreach (byte x in hashValue)
                {
                    sb.Append($"{x:x2}");
                }
                return sb.ToString();
            }
        }
    }
}
