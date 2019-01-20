using System;

namespace DAA.StateManagement.Interfaces
{
    public interface IStateManagementEventsAggregator<TData>
        where TData : IData
    {
        event EventHandler<IDescriptor> DataChangedEvent;
        event EventHandler<INonTerminalDescriptor> CompositionChangedEvent;

        void PublishDataChangedEvent(IDescriptor descriptor);
        void PublishCompositionChangedEvent(INonTerminalDescriptor descriptor);
    }
}
