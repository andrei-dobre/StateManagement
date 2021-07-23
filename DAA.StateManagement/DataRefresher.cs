using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DAA.StateManagement.Interfaces;

namespace DAA.StateManagement
{
    public class DataRefresher<TData> : IDataRefresher<TData>
        where TData: IData
    {
        public DataRefresher(IDataRetriever<TData> dataRetriever, IDataPool<TData> dataPool, IStateManagementEventsAggregator<TData> eventsAggregator)
        {
            EventsAggregator = eventsAggregator;
            DataRetriever = dataRetriever;
            DataPool = dataPool;
        }

        protected virtual IStateManagementEventsAggregator<TData> EventsAggregator { get; }

        protected virtual IDataRetriever<TData> DataRetriever { get; }

        protected virtual IDataPool<TData> DataPool { get; }

        public async Task RefreshAsync(IEnumerable<IDescriptor> descriptors)
        {
            var intersectingDescriptors = new HashSet<IDescriptor>(descriptors.SelectMany(a => DataPool.FindIntersectingDescriptors(a)));
            if (intersectingDescriptors.Count == 0) return;
            
            var refreshRetrievalContext = await DataRetriever.RefreshAsync(intersectingDescriptors);

            async Task DoRefreshAsync(IDescriptor descriptor)
            {
                await RefreshDataAsync(descriptor, refreshRetrievalContext);
            }

            await Task.WhenAll(intersectingDescriptors.Select(DoRefreshAsync).ToArray());
        }

        public virtual async Task RefreshAsync(IDescriptor descriptor)
        {
            var intersectingDescriptors = DataPool.FindIntersectingDescriptors(descriptor);

            await RefreshDataAsync(intersectingDescriptors);
        }

        public virtual async Task RefreshDataAsync(IEnumerable<IDescriptor> descriptors)
        {
            var tasksToRefreshDescriptors = descriptors.Select(RefreshDataAsync).ToArray();

            await Task.WhenAll(tasksToRefreshDescriptors);
        }

        public virtual async Task RefreshDataAsync(IDescriptor descriptor)
        {
            if (descriptor is ITerminalDescriptor terminalDescriptor)
            {
                await RefreshDataAsync(terminalDescriptor);
            }
            else
            {
                await RefreshDataAsync(descriptor as INonTerminalDescriptor);
            }
        }

        public virtual async Task RefreshDataAsync(ITerminalDescriptor descriptor)
        {
            await RefreshDataAsync(descriptor, await DataRetriever.RetrieveAsync(descriptor));
        }

        private async Task RefreshDataAsync(IDescriptor descriptor, IRefreshRetrievalContext<TData> refreshRetrievalContext)
        {
            if (descriptor is ITerminalDescriptor terminalDescriptor)
            {
                await RefreshDataAsync(terminalDescriptor,
                    refreshRetrievalContext.GetResult(terminalDescriptor));
            }
                
            if (descriptor is INonTerminalDescriptor nonTerminalDescriptor)
            {
                await RefreshDataAsync(nonTerminalDescriptor,
                    refreshRetrievalContext.GetResult(nonTerminalDescriptor));
            }
        }
        
        private async Task RefreshDataAsync(ITerminalDescriptor descriptor, IInstanceRetrievalContext<TData> freshData)
        {
            var instance = DataPool.Retrieve(descriptor);

            await DataPool.SaveAsync(descriptor, freshData);

            EventsAggregator.PublishDataChangedEvent(descriptor);
            EventsAggregator.PublishInstanceChangedEvent(new InstanceChangedEventArgs<TData>(descriptor, instance));
        }

        public virtual async Task RefreshDataAsync(INonTerminalDescriptor descriptor)
        {
            await RefreshDataAsync(descriptor, await DataRetriever.RetrieveCompositionAsync(descriptor));
        }

        private async Task RefreshDataAsync(INonTerminalDescriptor descriptor, IEnumerable<ITerminalDescriptor> freshComposition)
        {
            await UpdateCompositionAndAcquireAdditionsAsync(descriptor, freshComposition);

            EventsAggregator.PublishDataChangedEvent(descriptor);
            EventsAggregator.PublishCompositionChangedEvent(descriptor);
        }

        protected virtual async Task UpdateCompositionAndAcquireAdditionsAsync(INonTerminalDescriptor descriptor, IEnumerable<ITerminalDescriptor> freshComposition)
        {
            var additionsDescriptors = DataPool.UpdateCompositionAndProvideAdditions(descriptor, freshComposition);
            var additions = await DataRetriever.RetrieveAsync(additionsDescriptors);

            await DataPool.SaveAsync(additions);
        }
    }
}
