using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DAA.StateManagement.Interfaces;

namespace DAA.StateManagement
{
    public class DataRefresher<TData> : IDataRefresher<TData>
        where TData: IData
    {
        protected virtual IStateManagementEventsAggregator<TData> EventsAggregator { get; }
        protected virtual IDataRetriever<TData> DataRetriever { get; }
        protected virtual IDataPool<TData> DataPool { get; }


        public DataRefresher(IDataRetriever<TData> dataRetriever, IDataPool<TData> dataPool, IStateManagementEventsAggregator<TData> eventsAggregator)
        {
            EventsAggregator = eventsAggregator;
            DataRetriever = dataRetriever;
            DataPool = dataPool;
        }


        public virtual async Task RefreshAsync(IDescriptor descriptor)
        {
            var intersectingDescriptors = DataPool.FindIntersectingDescriptors(descriptor);

            await RefreshDataAsync(intersectingDescriptors);
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
            var freshData = await DataRetriever.RetrieveAsync(descriptor);

            DataPool.Save(descriptor, freshData);

            EventsAggregator.PublishDataChangedEvent(descriptor);
        }

        public virtual async Task RefreshDataAsync(INonTerminalDescriptor descriptor)
        {
            var freshComposition = await DataRetriever.RetrieveCompositionAsync(descriptor);

            await UpdateCompositionAndAcquireAdditionsAsync(descriptor, freshComposition);

            EventsAggregator.PublishDataChangedEvent(descriptor);
            EventsAggregator.PublishCompositionChangedEvent(descriptor);
        }

        public virtual async Task RefreshDataAsync(IEnumerable<IDescriptor> descriptors)
        {
            var tasksToRefreshDescriptors = descriptors.Select(RefreshDataAsync).ToArray();

            await Task.WhenAll(tasksToRefreshDescriptors);
        }

        protected virtual async Task UpdateCompositionAndAcquireAdditionsAsync(INonTerminalDescriptor descriptor, IEnumerable<ITerminalDescriptor> freshComposition)
        {
            var additionsDescriptors = DataPool.UpdateCompositionAndProvideAdditions(descriptor, freshComposition);
            var additions = await DataRetriever.RetrieveAsync(additionsDescriptors);

            DataPool.Save(additions);
        }
    }
}
