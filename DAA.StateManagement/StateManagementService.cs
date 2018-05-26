using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DAA.StateManagement.Interfaces;

namespace DAA.StateManagement
{
    public class StateManagementService<TData> : IStateManagementService<TData>
        where TData: IData
    {
        private IDataRetriever<TData> dataRetriever;
        private IDataPool<TData> dataPool;


        protected virtual IDataRetriever<TData> DataRetriever { get => dataRetriever; }
        protected virtual IDataPool<TData> DataPool { get => dataPool; }


        public StateManagementService(IDataRetriever<TData> dataRetriever, IDataPool<TData> dataPool)
        {
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
            var data = await this.DataRetriever.RetrieveAsync(descriptor);

            this.DataPool.Save(descriptor, data);
        }

        public virtual async Task RefreshDataAsync(INonTerminalDescriptor descriptor)
        { }
    }
}
