using Automatonymous;
using System;

namespace Masstventures
{
    public class StuffState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; }
        public Guid? ReadyTokenId { get; set; }

        public string Name { get; set; }
    }
}
