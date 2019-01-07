using System.Collections.Generic;
using Strava.Activities;

namespace Mvc.Client.Models
{
    public class ActivitiesModel
    {
        public string Token { get; set; }
        public IEnumerable<ActivitySummary> Activities { get; set; }
    }
}