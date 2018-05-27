using System;

namespace DAA.StateManagement.Interfaces
{
    public interface IStateEventsAggregator
    {
        event EventHandler<IDescriptor> DataChangedEvent;

        void PublishDataChangedEvent(IDescriptor descriptor, object sender);
    }
}
