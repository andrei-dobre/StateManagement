using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DAA.StateManagement.Interfaces;

namespace DAA.StateManagement
{
    public class StateManagementService<TData> : IStateManagementService<TData>
        where TData: IData
    {
        private IStateEventsAggregator stateEventsAggregator;
        private IDataRetriever<TData> dataRetriever;
        private IDataPool<TData> dataPool;


        protected virtual IStateEventsAggregator StateEventsAggregator { get => stateEventsAggregator; }
        protected virtual IDataRetriever<TData> DataRetriever { get => dataRetriever; }
        protected virtual IDataPool<TData> DataPool { get => dataPool; }


        public StateManagementService(IDataRetriever<TData> dataRetriever, IDataPool<TData> dataPool, IStateEventsAggregator stateEventsAggregator)
        {
            this.stateEventsAggregator = stateEventsAggregator;
            this.dataRetriever = dataRetriever;
            this.dataPool = dataPool;
        }


        public virtual async Task RefreshIntersectingDataAsync(IDescriptor descriptor)
        {
            var intersectingDescriptors = this.DataPool.FindIntersectingDescriptors(descriptor);
            await this.RefreshDataAsync(intersectingDescriptors);
        }

        public virtual async Task RefreshDataAsync(IEnumerable<IDescriptor> descriptors)
        {
            var tasksToRefreshIndividuallyDescribedData = descriptors.Select(_ => this.RefreshDataAsync(_)).ToArray();
            var taskToRefreshAllDescribedData = Task.WhenAll(tasksToRefreshIndividuallyDescribedData);

            await taskToRefreshAllDescribedData;
        }

        public virtual async Task RefreshDataAsync(IDescriptor descriptor)
        {
            if (descriptor is ITerminalDescriptor)
            {
                await this.RefreshDataAsync(descriptor as ITerminalDescriptor);
            }
            else
            {
                await this.RefreshDataAsync(descriptor as INonTerminalDescriptor);
            }
        }

        public virtual async Task RefreshDataAsync(ITerminalDescriptor descriptor)
        {
            var freshData = await this.DataRetriever.RetrieveAsync(descriptor);

            this.DataPool.Save(descriptor, freshData);
            this.PublishDataChangedEvent(descriptor);
        }

        public virtual async Task RefreshDataAsync(INonTerminalDescriptor descriptor)
        {
            var freshComposition = await this.DataRetriever.RetrieveCompositionAsync(descriptor);

            await this.UpdateCompositionAndAcquireAdditionsAsync(descriptor, freshComposition);
            this.PublishDataChangedEvent(descriptor);
        }

        protected virtual async Task UpdateCompositionAndAcquireAdditionsAsync(INonTerminalDescriptor descriptor, IEnumerable<ITerminalDescriptor> freshComposition)
        {
            var additionsDescriptors = this.DataPool.UpdateCompositionAndProvideAdditions(descriptor, freshComposition);
            var additions = await this.DataRetriever.RetrieveAsync(additionsDescriptors);
            this.DataPool.Save(additions);
        }


        private void PublishDataChangedEvent(IDescriptor descriptor)
        {
            this.StateEventsAggregator.PublishDataChangedEvent(descriptor, this);
        }
    }
}
