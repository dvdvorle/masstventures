using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MassTransit;
using System;
using Microsoft.Azure.Cosmos.Table;

namespace Masstventures
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            // Fill this in
            string serviceBusConnectionString = "";

            // Fill this in, make sure the tables 'StateMachine' and 'MessageAudit' exist.
            string cosmosDbConnectionString = "";

            return Host
                .CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton(s =>
                    {
                        var storageAccount = CloudStorageAccount.Parse(cosmosDbConnectionString);
                        return storageAccount.CreateCloudTableClient();
                    });
                    services.AddMassTransit(
                        x =>
                        {
                            x.SetAzureTableSagaRepositoryProvider(cfg => cfg.ConnectionFactory(s => s.GetRequiredService<CloudTableClient>().GetTableReference("StateMachine")));
                            x.AddServiceBusMessageScheduler();
                            x.UsingAzureServiceBus(
                                (context, cfg) =>
                                {
                                    cfg.UseAzureTableAuditStore(context.GetRequiredService<CloudTableClient>().GetTableReference("MessageAudit"));
                                    cfg.UseServiceBusMessageScheduler();

                                    cfg.Host(serviceBusConnectionString);
                                    
                                    // Uncomment this to make the schedule cancelling fail.
                                    //cfg.UseInMemoryOutbox();
                                    cfg.ConfigureEndpoints(context);
                                });
                            x.AddConsumers(typeof(Program).Assembly);
                            x.AddSagaStateMachines(typeof(Program).Assembly);
                        });
                    services.AddMassTransitHostedService(true);

                    services.AddHostedService<Worker>();
                });
        }
    }
}