using Microsoft.AspNetCore.Mvc;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Scheduler.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HolidaysController : ControllerBase
    {
        // GET: api/<HolidaysController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            var client = new RestClient("https://api.festdays.dev/v1/holidays?country=US&size=100&format=json&pretty=true&pretty=true&year=2021&key=56db283d18cf04931b2933396776f7610ee083");
            var request = new RestRequest(Method.GET);
            request.AddHeader("accept", "application/json");
            var response = client.Execute<IEnumerable<string>>(request);
            return response.Data;
        }

        // GET api/<HolidaysController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<HolidaysController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<HolidaysController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<HolidaysController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
