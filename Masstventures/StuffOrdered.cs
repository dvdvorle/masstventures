using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masstventures
{
    public record StuffOrdered(string Name, TimeSpan Delay) : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; set; }
    }
}
