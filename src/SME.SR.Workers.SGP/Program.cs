using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder
                    .UseKestrel()
                    .ConfigureAppConfiguration((hostingContext, config) =>
                       {
                           config.AddEnvironmentVariables();
                           config.AddUserSecrets<Program>();
                       })
                    .UseStartup<Startup>();
            });
    }
}
