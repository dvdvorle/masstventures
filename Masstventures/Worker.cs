using MassTransit;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Masstventures
{
    public class Worker : BackgroundService
    {
        private readonly IBus _bus;

        public Worker(IBus bus)
        {
            _bus = bus;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine("Pick your poison:");
                Console.WriteLine("1) Order stuff with 5 second delay");
                Console.WriteLine("2) Order stuff with 50 second delay");
                
                ConsoleKeyInfo option = default;
                
                await Task.Run(() => option = Console.ReadKey(true));
                switch (option.KeyChar)
                {
                    case '1':
                        await _bus.Publish(new StuffOrdered("1", TimeSpan.FromSeconds(5)), stoppingToken);
                        break;
                    case '2':
                        await _bus.Publish(new StuffOrdered("2", TimeSpan.FromSeconds(50)), stoppingToken);
                        break;
                }
            }
        }
    }
}
