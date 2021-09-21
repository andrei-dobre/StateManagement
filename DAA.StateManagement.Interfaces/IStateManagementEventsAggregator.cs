using System;

namespace DAA.StateManagement.Interfaces
{
    public interface IStateManagementEventsAggregator<TData>
        where TData : IData
    {
        event EventHandler<IDescriptor> DataChangedEvent;
        event EventHandler<INonTerminalDescriptor> CompositionChangedEvent;
        event EventHandler<InstanceChangedEventArgs<TData>> InstanceChangedEvent;
        event EventHandler<TerminalDataAvailableEventArgs<TData>> TerminalDataAvailableEvent;
        event EventHandler<NonTerminalDataAvailableEventArgs<TData>> NonTerminalDataAvailableEvent;

        void PublishDataChangedEvent(IDescriptor descriptor);
        void PublishCompositionChangedEvent(INonTerminalDescriptor descriptor);
        void PublishInstanceChangedEvent(InstanceChangedEventArgs<TData> args);
        void PublishTerminalDataAvailableEvent(ITerminalDescriptor descriptor, IInstanceRetrievalContext<TData> retrievalContext);
        void PublishNonTerminalDataAvailableEvent(INonTerminalDescriptor descriptor, ICollectionRetrievalContext<TData> retrievalContext);
    }
}
