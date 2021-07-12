using System.Collections.Generic;
using System.Threading.Tasks;
using DAA.StateManagement.Interfaces;

namespace DAA.StateManagement
{
    public class DataQualitySupervisor<TData> : IDataQualitySupervisor<TData>
        where TData : IData
    {
        public DataQualitySupervisor(IDataRefresher<TData> dataRefresher)
        {
            DataRefresher = dataRefresher;
        }

        public virtual IDataRefresher<TData> DataRefresher { get; }

        public async Task AcknowledgeStaleDataAsync(IEnumerable<IDescriptor> descriptors)
        {
            await DataRefresher.RefreshAsync(descriptors);
        }

        public async Task AcknowledgeStaleDataAsync(IDescriptor descriptor)
        {
            await DataRefresher.RefreshAsync(descriptor);
        }
    }
}
