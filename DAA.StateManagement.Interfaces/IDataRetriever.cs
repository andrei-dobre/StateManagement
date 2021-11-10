using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAA.StateManagement.Interfaces
{
    public interface IDataRetriever<TData>
        where TData : IData
    {
        Task<IRefreshRetrievalContext<TData>> RefreshAsync(IEnumerable<IDescriptor> descriptors);
        
        Task<IInstanceRetrievalContext<TData>> RetrieveAsync(ITerminalDescriptor descriptor);
        
        Task<ICollectionRetrievalContext<TData>> RetrieveAsync(IEnumerable<ITerminalDescriptor> descriptors);
        
        Task<ICollectionRetrievalContext<TData>> RetrieveAsync(INonTerminalDescriptor descriptor);
        
        Task<IEnumerable<ITerminalDescriptor>> RetrieveCompositionAsync(INonTerminalDescriptor descriptor);
    }
}
