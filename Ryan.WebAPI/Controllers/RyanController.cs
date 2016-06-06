using Ryan.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Ryan.WebAPI.Controllers
{
    public class RyanController : ApiController
    {
        // See the following websites for more info
        // - http://www.asp.net/web-api/overview/web-api-routing-and-actions/attribute-routing-in-web-api-2
        // - https://vivekcek.wordpress.com/tag/multiple-actions-were-found-that-match-the-request/
        // - http://stackoverflow.com/questions/11407267/multiple-httppost-method-in-web-api-controller
        // - http://stackoverflow.com/questions/13115004/multiple-actions-for-the-same-httpverb?rq=1  (this one helped me to know that the "action"-based route had to come first THEN the default route in the WebApiConfig.cs file

        [HttpGet]
        public ResponseStatus Get()
        {
            return new ResponseStatus { StatusCode = 0, StatusMessage = "Success!" };
        }

        [HttpPost]
        //[Route("api/ryan/SpecialPost")]
        public ResponseStatus SpecialPost([FromBody] dynamic value)
        {
            var response = string.Format("{0} added as new customer with salary of {1:C0}", value.Name.Value, (float)value.Salary.Value);
            return new ResponseStatus { StatusCode = 0, StatusMessage = response };
        }

        [HttpPost]
        //[Route("api/ryan/AnotherSpecialPost")]  // This method can be used if I don't want to add an entry into the WebApiConfig.cs file
        public ResponseStatus AnotherSpecialPost([FromBody]PersonRequest person)
        {
            var response = string.Format("{0} {1} added as new customer with salary of {2:C0}", person.FirstName, person.LastName, person.Salary);
            return new ResponseStatus { StatusCode = 0, StatusMessage = response };
        }

    }
}
