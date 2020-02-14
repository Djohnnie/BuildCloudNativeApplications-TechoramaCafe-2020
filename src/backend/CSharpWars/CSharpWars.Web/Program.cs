using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using static System.Environment;

namespace CSharpWars.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureAppConfiguration(configBuilder =>
                    {
                        var keyVault = GetEnvironmentVariable("KEY_VAULT");
                        var clientId = GetEnvironmentVariable("CLIENT_ID");
                        var clientSecret = GetEnvironmentVariable("CLIENT_SECRET");
                        configBuilder.AddAzureKeyVault(keyVault, clientId, clientSecret);
                    });
                    webBuilder.UseKestrel();
                    webBuilder.ConfigureKestrel((context, options) =>
                    {
                        var certificateKey = GetEnvironmentVariable("CERTIFICATE_KEY");
                        var certificateData = context.Configuration.GetValue<string>(certificateKey);
                        var serverCertificate = new X509Certificate2(Convert.FromBase64String(certificateData));
                        options.Listen(IPAddress.Any, 5000,
                            listenOptions => { listenOptions.UseHttps(serverCertificate); });
                    });
                    webBuilder.UseStartup<Startup>();
                });
    }
}