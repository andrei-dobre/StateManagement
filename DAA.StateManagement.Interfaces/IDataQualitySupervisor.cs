using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAA.StateManagement.Interfaces
{
    public interface IDataQualitySupervisor<TData>
        where TData : IData
    {
        Task AcknowledgeStaleDataAsync(IEnumerable<IDescriptor> descriptors);
        
        Task AcknowledgeStaleDataAsync(IDescriptor descriptor);
    }
}
