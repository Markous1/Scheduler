using Microsoft.AspNetCore.Authorization;
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
        [Authorize]
        public string Get()
        {
            var client = new RestClient("https://api.festdays.dev/v1/holidays?country=US&size=100&format=json&pretty=true&pretty=true&year=2021&key=56db283d18cf04931b2933396776f7610ee083");
            var request = new RestRequest(Method.GET);
            //request.AddHeader("token", "56db283d18cf04931b2933396776f7610ee083");
            //<IEnumerable<string>>
            request.AddHeader("accept", "application/json");
            IRestResponse response = client.Execute(request);
            return response.Content;
        }

        // GET: api/<HolidaysController>/date
        [HttpGet("{date}")]
        [Authorize]
        public string Get(int date)
        {
            var client = new RestClient("https://api.festdays.dev/v1/holidays?year=2021&before=2021-12-01&after=2021-12-31&pretty=true&key=56db283d18cf04931b2933396776f7610ee083");
            var request = new RestRequest(Method.GET);
            request.AddHeader("accept", "application/json");
            IRestResponse response = client.Execute(request);
            return response.Content;
        }

        // GET api/<HolidaysController>/region/country
        [HttpGet("{region}/{country}")]
        [Authorize]
        public string Get(string region, string country)
        {
            var client = new RestClient("https://api.festdays.dev/v1/holidays?region=CA&country=US&year=2021&pretty=true&key=56db283d18cf04931b2933396776f7610ee083");
            var request = new RestRequest(Method.GET);
            request.AddHeader("accept", "application/json");
            var response = client.Execute(request);
            return response.Content;
        }

        //// GET api/<HolidaysController>
        //[HttpGet("{id}")]
        //[Authorize]
        //public string Get(int id)
        //{
        //    return "value";
        //}

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
