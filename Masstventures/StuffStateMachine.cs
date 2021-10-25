using Automatonymous;
using System;

namespace Masstventures
{
    public class StuffStateMachine : MassTransitStateMachine<StuffState>
    {
        public StuffStateMachine()
        {
            InstanceState(x => x.CurrentState);
            Schedule(
                () => StuffReady,
                instance => instance.ReadyTokenId,
                s =>
                {
                    s.Received = r => r.CorrelateById(c => c.Message.CorrelationId);
                });

            Initially(
                When(StuffOrderedLater)
                    .Then(x => x.Instance.Name = x.Data.Name)
                    .Schedule(StuffReady, x => new StuffReady { CorrelationId = x.Data.CorrelationId }, x => x.Data.Delay)
                    .TransitionTo(Idle));

            During(Idle,
                When(StuffOrderedLater)
                    .Then(x => x.Instance.Name = x.Data.Name)
                    .Schedule(StuffReady, x => new StuffReady { CorrelationId = x.Data.CorrelationId }, x => x.Data.Delay),
                When(StuffReady.Received)
                    .Publish(x => new StuffMessage($"Hey, stuff '{x.Instance.Name}' ordered!")));
        }

        public State Idle { get; set; }

        public Event<StuffOrdered> StuffOrderedLater { get; private set; }
        
        public Schedule<StuffState, StuffReady> StuffReady { get; private set; }
    }
}
