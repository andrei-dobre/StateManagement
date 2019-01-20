using System;
using DAA.StateManagement.Interfaces;

namespace DAA.StateManagement
{
    public class StateManagementEventsAggregator<TData> : IStateManagementEventsAggregator<TData>
        where TData : IData
    {
        public event EventHandler<IDescriptor> DataChangedEvent;
        public event EventHandler<INonTerminalDescriptor> CompositionChangedEvent;


        public void PublishDataChangedEvent(IDescriptor descriptor)
        {
            DataChangedEvent?.Invoke(this, descriptor);
        }

        public void PublishCompositionChangedEvent(INonTerminalDescriptor descriptor)
        {
            CompositionChangedEvent?.Invoke(this, descriptor);
        }
    }
}
