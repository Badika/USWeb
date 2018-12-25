using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace UpsCoolWeb.Web
{
    public class Program
    {
        public static void Main()
        {
            new WebHostBuilder()
                .UseKestrel(options => options.AddServerHeader = false)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .UseIISIntegration()
                .UseIIS()
                .Build()
                .Run();
        }
    }
}
