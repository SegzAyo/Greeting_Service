using GreetingService.API.Function.Authentication;
using GreetingService.Core;
using GreetingService.Infrastructure;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

[assembly: FunctionsStartup(typeof(GreetingService.API.Function.Startup))]
namespace GreetingService.API.Function
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            //builder.Services.AddHttpClient();

            var config = builder.GetContext().Configuration;
            var connectionString = config["LoggingStorageAccount"];

            builder.Services.AddLogging(c =>
            {
                if (string.IsNullOrWhiteSpace(connectionString))
                    return;
                var logName = $"{Assembly.GetCallingAssembly().GetName().Name}.log";
                var logger = new LoggerConfiguration()
                                    .WriteTo.AzureBlobStorage(connectionString,
                                                              restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
                                                              storageFileName: "{yyyy}/{MM}/{dd}/" + logName,
                                                              outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] [{SourceContext}] {Message}{NewLine}{Exception}")
                                    .CreateLogger();

                c.AddSerilog(logger, true);
            });



            //builder.Services.AddScoped<IGreetingRepository, FileGreetingRepository>(c =>
            //{
            //    var config = c.GetService<IConfiguration>();
            //    return new FileGreetingRepository(config["FileRepositoryFilePath"]);
            //});

            builder.Services.AddDbContext<GreetingDbContext>(options =>
            {
                options.UseSqlServer(config["GreetingDbConnectionString"]);
            });

            builder.Services.AddScoped<IGreetingRepository, SqlGreetingRepository>();
            //builder.Services.AddScoped<IGreetingRepository, BlobGreetingRepository>();
            builder.Services.AddScoped<IUserService, SqlUserService>();
            builder.Services.AddScoped<IAuthHandler, BasicAuthHandler>();



        }
    }
}