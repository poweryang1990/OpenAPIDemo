using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using OpenAPI.Core;
using OpenAPI.Core.Filter;

namespace OpenAPI.Controllers
{
    [InnerSignAuthentication]
    public class ValuesController : ApiController
    {
      
        
        [HttpGet]
        public string Show(string name,int age)
        {
            return $"{name}:{age}";
        }

        // POST api/values
        [HttpPost]
        public string Add(User user)
        {
            return $"{user?.Name}:{user?.Age}";
        }

        
    }

    public class User
    {
        public string Name { get; set; }

        public  int Age { get; set; }
    }
}
