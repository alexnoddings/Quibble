using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Quibble.Server
{
    /// <summary>
    /// Entry point for the program.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Entry point for the program.
        /// </summary>
        /// <param name="args">Command-line arguments.</param>
        public static void Main(string[] args) =>
            CreateHostBuilder(args).Build().Run();

        /// <summary>
        /// Creates a host builder to run the program.
        /// </summary>
        /// <param name="args">Command-line arguments.</param>
        /// <returns>The <see cref="IHostBuilder"/> for the program.</returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
    }
}
