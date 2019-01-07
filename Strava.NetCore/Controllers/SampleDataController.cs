using System;
using System.Net.Http;
using System.Web.Http;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Strava.Activities;
using Strava.Authentication;
using Strava.Clients;
using Newtonsoft.Json;

namespace Strava.NetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SampleDataController : Controller
    {
        private static string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        [HttpGet("[action]")]
        public IEnumerable<WeatherForecast> WeatherForecasts()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                DateFormatted = DateTime.Now.AddDays(index).ToString("d"),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            });
        }

        [HttpGet("[action]")]
        public ActionResult<IEnumerable<ActivitySummary>> GetActivities()
        {
            WebAuthentication wa = new WebAuthentication();
            wa.AccessToken = User.Claims.First(claim => claim.Type == "token").Value;
            var activities = new StravaClient(wa).Activities.GetActivitiesAsync(40, 40).Result;
            if (activities == null)
            {
                return NotFound();
            }
            return activities;
        }

        public class WeatherForecast
        {
            public string DateFormatted { get; set; }
            public int TemperatureC { get; set; }
            public string Summary { get; set; }

            public int TemperatureF
            {
                get
                {
                    return 32 + (int)(TemperatureC / 0.5556);
                }
            }
        }
    }
}
