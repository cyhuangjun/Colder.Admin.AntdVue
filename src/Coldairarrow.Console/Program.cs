using Autofac;
using Autofac.Extensions.DependencyInjection;
using Coldairarrow.Scheduler.Core;
using Coldairarrow.Util;
using EFCore.Sharding;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using Colder.Logging.Serilog;
using Coldairarrow.Api;

namespace Coldairarrow.Scheduler
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args)
                .ConfigureLoggingDefaults()
                .UseIdHelper()
                .UseCache()
                .ConfigureServices((hostContext, services) =>
                {
                    //services.AddLogging();
                    services.AddFxServices();
                    services.AddAutoMapper();
                    services.AddEFCoreSharding(config =>
                    {
                        var dbOptions = hostContext.Configuration.GetSection("Database:BaseDb").Get<DatabaseOptions>();
                        config.UseDatabase(dbOptions.ConnectionString, dbOptions.DatabaseType);
                    });
                    services.AddSingleton<IScheduler, DefaultScheduler>();
                    services.AddAutofac();
                })
                .ConfigureContainer<ContainerBuilder>(builder =>
                {
                    builder.RegisterType<CustomHostService>()
                           .As<IHostedService>()
                           .InstancePerDependency();
                    builder.RegisterModule(new RmesAutoFacModule());
                })
                .UseServiceProviderFactory<ContainerBuilder>(new AutofacServiceProviderFactory())
                .UseConsoleLifetime(); 
            var host = builder.Build();  
            await host.RunAsync();
        }
    }
}