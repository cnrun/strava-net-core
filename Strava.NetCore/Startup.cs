using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
// Imports the Google Cloud KMS client library
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.CloudKMS.v1;
using Google.Apis.CloudKMS.v1.Data;
using System;

namespace Strava.NetCore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.LoginPath = "/login";
                options.LogoutPath = "/signout";
            })
            .AddStrava(options =>
            {
                // ClientId and CleintSecret can be configured in your appsettings.json file
                // ....
                //  "Strava": {
                //    "ClientId": "5275",
                //    "ClientSecret": "5930e45f6727e4656eb830e7a3893efbcef2a37b"
                //  },
                // ....
                // Your Google Cloud Platform project ID.
                string projectId = "jenkins-x-002";

                // Lists keys in the "global" location.
                string location = "global";

                // The resource name of the location associated with the key rings.
                string parent = $"projects/{projectId}/locations/{location}";

                // Authorize the client using Application Default Credentials.
                // See: https://developers.google.com/identity/protocols/application-default-credentials
                GoogleCredential credential = GoogleCredential.GetApplicationDefaultAsync().Result;
                // Specify the Cloud Key Management Service scope.
                if (credential.IsCreateScopedRequired)
                {
                    credential = credential.CreateScoped(new[]
                    {
                    Google.Apis.CloudKMS.v1.CloudKMSService.Scope.CloudPlatform
                });
                }
                // Instantiate the Cloud Key Management Service API.
                CloudKMSService cloudKms = new CloudKMSService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    GZipEnabled = false
                });
                string keyRingId = "oauth_keystore";
                string cryptoKeyId = "strava_client_secret";

                var ClientIdEnc = Configuration["Strava:ClientId"];
                options.ClientId = Decrypt(cloudKms, projectId, location, keyRingId, cryptoKeyId, ClientIdEnc);

                var ClientSecretEnc = Configuration["Strava:ClientSecret"];
                options.ClientSecret = Decrypt(cloudKms, projectId, location, keyRingId, cryptoKeyId, ClientSecretEnc);
            });
        }

        public static string Decrypt(CloudKMSService cloudKms, string projectId, string locationId, string keyRingId, string cryptoKeyId, string ciphertext)
        {
            //var cloudKms = CreateAuthorizedClient();
            // Generate the full path of the crypto key to use for encryption.
            var cryptoKey = $"projects/{projectId}/locations/{locationId}/keyRings/{keyRingId}/cryptoKeys/{cryptoKeyId}";
            DecryptRequest decryptRequest = new DecryptRequest();
            decryptRequest.Ciphertext = ciphertext;
            Console.WriteLine($"dataToDecrypt.Ciphertext: {decryptRequest.Ciphertext}");
            var result = cloudKms.Projects.Locations.KeyRings.CryptoKeys.Decrypt(name: cryptoKey, body: decryptRequest).Execute();
            // Output decrypted data to a file.
            var plaintext = result.Plaintext;
            Console.WriteLine($"Decrypted file created: {plaintext}");
            return plaintext;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            if (false)
            {
                app.UseStaticFiles();
                app.UseAuthentication();
                app.UseMvc();
            }
            else
            {
                app.UseStaticFiles();
                app.UseSpaStaticFiles();
                app.UseAuthentication();
                app.UseMvc(routes =>
                {
                    routes.MapRoute(
                        name: "default",
                        template: "{controller}/{action=Index}/{id?}");
                });

                app.UseSpa(spa =>
                {
                    spa.Options.SourcePath = "ClientApp";

                    if (env.IsDevelopment())
                    {
                        spa.UseReactDevelopmentServer(npmScript: "start");
                    }
                });
            }
        }
    }
}
