using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Adub.SignalR.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace Adub.SignalR
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // Not sure why, but seem to need to configure corspolicybuilder here as well as
            // in Configure method.  Adding same builder configuration.
            services.AddCors(options => options.AddPolicy("CorsPolicy", builder =>
            {
                builder.AllowAnyHeader()
                        .AllowAnyMethod()
                        .WithOrigins("https://adub-signalr-webclient.apps.pcf.nonprod.cudirect.com")
                        .AllowCredentials();
            }));

            // Add distributed cache which is required for session state
            services.AddDistributedMemoryCache();

            // Add session service and specify JSESSIONID for cookie name
            // This is used during negotiation phase for sticky sessions on PCF
            // Not ideal, but seems to be required by SignalR for negotiation
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.Name = "JSESSIONID";
            });

            // Add SignalR service with redis backplane
            services.AddSignalR().AddRedis(GetRedisConnectionString());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // Not sure why, but seem to need to configure corspolicybuilder here as well as
            // in ConfigureServices method. Adding same builder configuration.
            app.UseCors(builder =>
            {
                builder.AllowAnyHeader()
                        .AllowAnyMethod()
                        .WithOrigins("https://adub-signalr-webclient.apps.pcf.nonprod.cudirect.com")
                        .AllowCredentials();
            });

            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseExceptionHandler("/Error");

            // Use session service
            app.UseSession();

            // Custom use method to add a value to the session cookie
            app.Use(async (context, next) =>
            {
                var sessionGuid = Guid.NewGuid().ToString();
                Console.WriteLine("Setting sessionid " + sessionGuid);
                context.Session.SetString("JSESSIONID", sessionGuid);
                await next();
            });

            // Map SignalR routes
            app.UseSignalR(routes =>
            {
                routes.MapHub<NotificationHub>("/notificationhub");
            });
        }

        // Retreive connection string to PCF managed Redis cache
        private string GetRedisConnectionString()
        {
            var credentials = "$..[?(@.name=='signalr-sync-cache')].credentials";
            var jObj = JObject.Parse(Environment.GetEnvironmentVariable("VCAP_SERVICES"));

            if (jObj.SelectToken($"{credentials}") == null)
                throw new Exception("Expects a PCF managed redis cache service binding named 'signalr-sync-cache'");

            var host = (string)jObj.SelectToken($"{credentials}.host");
            var pwd = (string)jObj.SelectToken($"{credentials}.password");
            var port = (string)jObj.SelectToken($"{credentials}.port");

            Console.Out.WriteLine($"{host}:{port},password={pwd},allowAdmin=true");

            return $"{host}:{port},password={pwd},allowAdmin=true";
        }
    }
}
