using System.Threading.Tasks;
using CSharpWars.Common.DependencyInjection;
using CSharpWars.Processor.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using static System.Environment;

namespace CSharpWars.Processor
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).RunConsoleAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(configBuilder =>
                {
                    var keyVault = GetEnvironmentVariable("KEY_VAULT");
                    var clientId = GetEnvironmentVariable("CLIENT_ID");
                    var clientSecret = GetEnvironmentVariable("CLIENT_SECRET");
                    configBuilder.AddAzureKeyVault(keyVault, clientId, clientSecret);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.ConfigurationHelper(c =>
                    {
                        c.ConnectionString = hostContext.Configuration.GetValue<string>("connection-string");
                        c.ArenaSize = hostContext.Configuration.GetValue<int>("arena-size");
                    });
                    services.ConfigureScriptProcessor();
                    services.AddHostedService<Worker>();
                });
    }
}