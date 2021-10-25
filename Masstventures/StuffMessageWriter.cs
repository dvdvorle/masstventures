using MassTransit;
using System;
using System.Threading.Tasks;

namespace Masstventures
{
    public class StuffMessageWriter : IConsumer<StuffMessage>
    {
        public Task Consume(ConsumeContext<StuffMessage> context)
        {
            Console.WriteLine(context.Message.Text);
            return Task.CompletedTask;
        }
    }
}