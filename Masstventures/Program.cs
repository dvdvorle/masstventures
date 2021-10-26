using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MassTransit;

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

            return Host
                .CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddMassTransit(
                        x =>
                        {
                            x.SetInMemorySagaRepositoryProvider();
                            x.AddServiceBusMessageScheduler();
                            x.UsingAzureServiceBus(
                                (context, cfg) =>
                                {
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