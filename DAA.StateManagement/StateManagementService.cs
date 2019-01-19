using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DAA.StateManagement.Interfaces;

namespace DAA.StateManagement
{
    public class StateManagementService<TData> : IStateManagementService<TData>
        where TData: IData
    {
        protected virtual IStateEventsAggregator StateEventsAggregator { get; }
        protected virtual IDataRetriever<TData> DataRetriever { get; }
        protected virtual IDataPool<TData> DataPool { get; }


        public StateManagementService(IDataRetriever<TData> dataRetriever, IDataPool<TData> dataPool, IStateEventsAggregator stateEventsAggregator)
        {
            StateEventsAggregator = stateEventsAggregator;
            DataRetriever = dataRetriever;
            DataPool = dataPool;
        }


        public virtual async Task RefreshIntersectingDataAsync(IDescriptor descriptor)
        {
            var intersectingDescriptors = DataPool.FindIntersectingDescriptors(descriptor);
            await RefreshDataAsync(intersectingDescriptors);
        }

        public virtual async Task RefreshDataAsync(IEnumerable<IDescriptor> descriptors)
        {
            var tasksToRefreshIndividuallyDescribedData = descriptors.Select(RefreshDataAsync).ToArray();
            var taskToRefreshAllDescribedData = Task.WhenAll(tasksToRefreshIndividuallyDescribedData);

            await taskToRefreshAllDescribedData;
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
            PublishDataChangedEvent(descriptor);
        }

        public virtual async Task RefreshDataAsync(INonTerminalDescriptor descriptor)
        {
            var freshComposition = await DataRetriever.RetrieveCompositionAsync(descriptor);

            await UpdateCompositionAndAcquireAdditionsAsync(descriptor, freshComposition);
            PublishDataChangedEvent(descriptor);
        }

        protected virtual async Task UpdateCompositionAndAcquireAdditionsAsync(INonTerminalDescriptor descriptor, IEnumerable<ITerminalDescriptor> freshComposition)
        {
            var additionsDescriptors = DataPool.UpdateCompositionAndProvideAdditions(descriptor, freshComposition);
            var additions = await DataRetriever.RetrieveAsync(additionsDescriptors);
            DataPool.Save(additions);
        }


        private void PublishDataChangedEvent(IDescriptor descriptor)
        {
            StateEventsAggregator.PublishDataChangedEvent(descriptor, this);
        }
    }
}
