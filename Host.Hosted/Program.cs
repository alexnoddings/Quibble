using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Hosting = Microsoft.Extensions.Hosting.Host;

namespace Quibble.Host.Hosted
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Hosting.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}