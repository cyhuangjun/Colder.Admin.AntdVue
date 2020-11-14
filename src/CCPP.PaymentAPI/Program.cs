using Coldairarrow.Util;
using Colder.Logging.Serilog;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace CCPP.PaymentAPI
{
    public class Program
    {
        public static void Main(string[] args)
        { 
            Host.CreateDefaultBuilder(args)
                .ConfigureLoggingDefaults()
                .UseIdHelper()
                .UseCache()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseUrls("http://*:8000")
                        .UseStartup<Startup>();
                })
                .Build()
                .Run();
        } 
    }
}
