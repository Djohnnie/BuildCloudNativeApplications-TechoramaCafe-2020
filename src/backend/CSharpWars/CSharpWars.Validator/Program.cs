using System;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace CSharpWars.Validator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var certificateFileName = Environment.GetEnvironmentVariable("CERTIFICATE_FILENAME");
            var certificatePassword = Environment.GetEnvironmentVariable("CERTIFICATE_PASSWORD");
            CreateHostBuilder(args, certificateFileName, certificatePassword).Build().Run();
        }

        // Additional configuration is required to successfully run gRPC on macOS.
        // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
        public static IHostBuilder CreateHostBuilder(string[] args, String certificateFileName, String certificatePassword) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel();
                    webBuilder.ConfigureKestrel((context, options) =>
                    {
                        options.Listen(IPAddress.Any, 5555,
                            listenOptions => { listenOptions.UseHttps(certificateFileName, certificatePassword); });
                    });
                    webBuilder.UseStartup<Startup>();
                });
    }
}