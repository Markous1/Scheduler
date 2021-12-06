using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduler.Controllers
{
    [Route("api")]
    [ApiController]
    public class ApiController0 : ControllerBase
    {
        #region experiments
        /* This code is for testing only, and should NOT be included in final product
         */
        [Route("test")]
        public string Foo() { return "hello world!"; }
        #endregion
    }
}
