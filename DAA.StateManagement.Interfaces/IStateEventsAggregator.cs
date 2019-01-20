using System;

namespace DAA.StateManagement.Interfaces
{
    public interface IStateEventsAggregator
    {
        event EventHandler<IDescriptor> DataChangedEvent;
        event EventHandler<INonTerminalDescriptor> CompositionChangedEvent;

        void PublishDataChangedEvent(IDescriptor descriptor);
        void PublishCompositionChangedEvent(INonTerminalDescriptor descriptor);
    }
}
