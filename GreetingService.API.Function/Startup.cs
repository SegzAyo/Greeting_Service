using Azure.Messaging.ServiceBus;
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
using GreetingService.Infrastructure;
using Azure.Identity;
using Microsoft.Extensions.Hosting;

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
            builder.Services.AddScoped<IInvoiceService, SqlInvoiceService>();
            builder.Services.AddScoped<IAuthHandler, BasicAuthHandler>();
            builder.Services.AddScoped<IMessagingService, ServiceBusMessagingService>();
            builder.Services.AddScoped<IApprovalService, TeamsApprovalService>();




            builder.Services.AddSingleton(c =>
            {
                var serviceBusClient = new ServiceBusClient(config["ServiceBusConnectionString"]);      //remember to add this connection to the application configuration
                return serviceBusClient.CreateSender("main");
            });


        }
        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            var builtConfig = builder.ConfigurationBuilder.Build();
            var keyVaultEndpoint = builtConfig["AzureKeyVaultEndpoint"];

            if (!string.IsNullOrEmpty(keyVaultEndpoint))
            {
                // might need this depending on local dev env
                //var credential = new DefaultAzureCredential(
                //    new DefaultAzureCredentialOptions { ExcludeSharedTokenCacheCredential = true });

                // using Key Vault, either local dev or deployed
                builder.ConfigurationBuilder

                        .AddAzureKeyVault(keyVaultEndpoint);
                       
            }
            else
            {
                // local dev no Key Vault
                builder.ConfigurationBuilder
                   .SetBasePath(Environment.CurrentDirectory)
                   .AddJsonFile("local.settings.json", true)
                   .AddUserSecrets(Assembly.GetExecutingAssembly(), true)
                   .AddEnvironmentVariables()
                   .Build();
            }
        }
    }
}