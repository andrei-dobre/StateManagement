using System;
using DAA.StateManagement.Interfaces;

namespace DAA.StateManagement
{
    public class StateManagementEventsAggregator<TData> : IStateManagementEventsAggregator<TData>
        where TData : IData
    {
        public event EventHandler<IDescriptor> DataChangedEvent;
        public event EventHandler<INonTerminalDescriptor> CompositionChangedEvent;
        public event EventHandler<InstanceChangedEventArgs<TData>> InstanceChangedEvent;
        public event EventHandler<TerminalDataAvailableEventArgs<TData>> TerminalDataAvailableEvent;
        public event EventHandler<NonTerminalDataAvailableEventArgs<TData>> NonTerminalDataAvailableEvent;


        public void PublishDataChangedEvent(IDescriptor descriptor)
        {
            DataChangedEvent?.Invoke(this, descriptor);
        }

        public void PublishCompositionChangedEvent(INonTerminalDescriptor descriptor)
        {
            CompositionChangedEvent?.Invoke(this, descriptor);
        }

        public void PublishInstanceChangedEvent(InstanceChangedEventArgs<TData> args)
        {
            InstanceChangedEvent?.Invoke(this, args);
        }

        public void PublishTerminalDataAvailableEvent(ITerminalDescriptor descriptor, IInstanceRetrievalContext<TData> retrievalContext)
        {
            TerminalDataAvailableEvent?.Invoke(this, new TerminalDataAvailableEventArgs<TData>(descriptor, retrievalContext));
        }

        public void PublishNonTerminalDataAvailableEvent(INonTerminalDescriptor descriptor,
            ICollectionRetrievalContext<TData> retrievalContext)
        {
            NonTerminalDataAvailableEvent?.Invoke(this, new NonTerminalDataAvailableEventArgs<TData>(descriptor, retrievalContext));
        }
    }
}
