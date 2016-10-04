using Ryan.AngularJS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Ryan.AngularJS.Controllers
{
    public class TestApiController : ApiController
    {

        [HttpPost]
        //[Route("api/TestApi/SearchByApn")]  // This method can be used if I don't want to add an entry into the WebApiConfig.cs file
        public AjaxResponse SearchByApn([FromBody] dynamic inData)
        {
            //var response = string.Format("{0} {1} added as new customer with salary of {2:C0}", person.FirstName, person.LastName, person.Salary);
            return new AjaxResponse { StatusMessage = "Good job", MulitplePropertiesFound = false };
        }

    }
}
