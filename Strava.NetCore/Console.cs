using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Strava.Activities;
using Strava.Athletes;
using Strava.Authentication;
using Strava.Clients;

namespace StravaCode
{
    class Program
    {
        static void Main0(string[] args)
        {
            Console.WriteLine("Start LocalWebServer");
            LocalWebServer lws = new LocalWebServer($"http://{args[0]}:1895/");
            lws.Start();
            WebAuthentication wa = new WebAuthentication();
            string clientId = args[1];
            string clientSecret = args[2];
            Scope scope = Scope.Public;
            int callbackPort = 1895;
            wa.AccessTokenReceived += c_AccessTokenReceived;
            wa.AuthCodeReceived += c_AuthCodeReceived;
            wa.GetTokenAsync(clientId, clientSecret, scope, callbackPort);

            Console.WriteLine("Wait Token");
            Task.Delay(10000).Wait();
            Console.WriteLine("Query");

            Console.WriteLine($"WebAuthentication {wa.AccessToken} {wa.AuthCode}");

            var a = MyMethodAsync(wa);
            Task.WaitAll(a); //Now Waiting      
            Console.WriteLine("Wait before quit");
            Task.Delay(5000).Wait();
            lws.Stop();
            Console.WriteLine("Exiting CommandLine");
        }
        static void c_AuthCodeReceived(object sender, AuthCodeReceivedEventArgs e)
        {
            Console.WriteLine($"AuthCode received {e.AuthCode} sender {sender}");
        }
        static void c_AccessTokenReceived(object sender, TokenReceivedEventArgs e)
        {
            Console.WriteLine($"Token received {e.Token} sender {sender}");
        }

        public static async Task MyMethodAsync(WebAuthentication wa)
        {
            StravaClient client = new StravaClient(wa);
            // Receive the currently authenticated athlete
            List<ActivitySummary> activities = await client.Activities.GetActivitiesAsync(20, 20);

            foreach (ActivitySummary Activity in activities)
            {
                Console.WriteLine($"Email:{Activity.Name} {Activity.Type}");
            }
        }
    }
}
