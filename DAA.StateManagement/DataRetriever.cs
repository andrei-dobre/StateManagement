using System.Collections.Generic;
using System.Threading.Tasks;
using DAA.StateManagement.Interfaces;

namespace DAA.StateManagement
{
    public abstract class DataRetriever<TData> : IDataRetriever<TData>
        where TData : IData
    {
        public DataRetriever(ITerminalDescriptorsFactory<TData> terminalDescriptorsFactory, 
            IStateManagementEventsAggregator<TData> eventsAggregator)
        {
            TerminalDescriptorsFactory = terminalDescriptorsFactory;
            EventsAggregator = eventsAggregator;
        }

        protected ITerminalDescriptorsFactory<TData> TerminalDescriptorsFactory { get; }
        
        protected IStateManagementEventsAggregator<TData> EventsAggregator { get; }

        public abstract Task<IRefreshRetrievalContext<TData>> RefreshAsync(IEnumerable<IDescriptor> descriptors);
        
        public abstract Task<IInstanceRetrievalContext<TData>> RetrieveAsync(ITerminalDescriptor descriptor);
        
        public abstract Task<ICollectionRetrievalContext<TData>> RetrieveAsync(IEnumerable<ITerminalDescriptor> descriptors);
        
        public abstract Task<ICollectionRetrievalContext<TData>> RetrieveAsync(INonTerminalDescriptor descriptor);
        
        public abstract Task<IEnumerable<ITerminalDescriptor>> RetrieveCompositionAsync(INonTerminalDescriptor descriptor);
    }
}
