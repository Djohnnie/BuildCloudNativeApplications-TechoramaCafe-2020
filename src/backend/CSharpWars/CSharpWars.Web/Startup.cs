using System;
using CSharpWars.Common.DependencyInjection;
using CSharpWars.Web.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CSharpWars.Web
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
            services.ConfigurationHelper(c =>
            {
                c.ConnectionString = Configuration.GetValue<string>("connection-string");
                c.ArenaSize = Convert.ToInt32(Environment.GetEnvironmentVariable("ARENA_SIZE"));
                c.ValidationHost = Environment.GetEnvironmentVariable("VALIDATION_HOST");
                c.PointsLimit = Convert.ToInt32(Environment.GetEnvironmentVariable("POINTS_LIMIT"));
                c.BotDeploymentLimit = Convert.ToInt32(Environment.GetEnvironmentVariable("DEPLOYMENT_LIMIT"));
            });

            services.ConfigureWeb();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseSession();
            app.UseRouting();
            app.UseCookiePolicy();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
                endpoints.MapGet("/status", async context =>
                {
                    await context.Response.WriteAsync(string.Empty);
                });
            });
        }
    }
}