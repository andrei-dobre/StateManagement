using System.Threading.Tasks;
using DAA.StateManagement.Interfaces;

namespace DAA.StateManagement
{
    public class DataQualitySupervisor<TData> : IDataQualitySupervisor<TData>
        where TData : IData
    {
        public virtual IDataRefresher<TData> DataRefresher { get; }


        public DataQualitySupervisor(IDataRefresher<TData> dataRefresher)
        {
            DataRefresher = dataRefresher;
        }


        public async Task AcknowledgeStaleDataAsync(IDescriptor descriptor)
        {
            await DataRefresher.RefreshAsync(descriptor);
        }
    }
}
