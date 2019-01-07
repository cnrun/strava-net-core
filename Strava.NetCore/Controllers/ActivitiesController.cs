/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers
 * for more information concerning the license and the contributors participating to this project.
 */

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Strava.Activities;
using Strava.Athletes;
using Strava.Authentication;
using Strava.Clients;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using Mvc.Client.Models;

namespace Mvc.Client.Controllers
{
    [Authorize]
    public class ActivitiesController : Controller
    {
        public ActivitiesController()
        {            
        }

        [HttpGet("~/activities"), HttpPost("~/activities")]
        public ActionResult Activities()
        {
            // var model=new string[]{"ABCD","EFGH"};
            ActivitiesModel model = new ActivitiesModel();
            var token = User.Claims.First(claim=>claim.Type=="token").Value;
            model.Token=token;
            WebAuthentication wa = new WebAuthentication();
            wa.AccessToken=token;
            Task<IEnumerable<ActivitySummary>> t = GetActivityListAsync(wa);


            IEnumerable<ActivitySummary> activities = t.Result;
            model.Activities=activities;
            return View(model);
        }
        
        [HttpGet("~/api/[controller]/[action]")]
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

        private async Task<IEnumerable<ActivitySummary>> GetActivityListAsync(WebAuthentication wa)
        {
            return await new StravaClient(wa).Activities.GetActivitiesAsync(40, 40);
        }
    }
}
