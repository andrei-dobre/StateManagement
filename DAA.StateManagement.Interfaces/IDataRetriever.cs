using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAA.StateManagement.Interfaces
{
    public interface IDataRetriever<TData>
        where TData : IData
    {
        Task<TData> RetrieveAsync(ITerminalDescriptor descriptor);


        Task<IEnumerable<TData>> RetrieveAsync(IEnumerable<ITerminalDescriptor> descriptors);

        Task<IEnumerable<TData>> RetrieveAsync(INonTerminalDescriptor descriptor);


        Task<IEnumerable<ITerminalDescriptor>> RetrieveCompositionAsync(INonTerminalDescriptor descriptor);
    }
}
