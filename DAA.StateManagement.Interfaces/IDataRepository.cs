using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAA.StateManagement.Interfaces
{
    public interface IDataRepository<TData> : ICollectionsManager<TData>
        where TData : IData
    {
        Task<TData> RetrieveAsync(ITerminalDescriptor descriptor);
        
        Task<TData> RetrieveAsync(ITerminalDescriptor descriptor, IDataBuilder<TData> builder);
        
        Task<IEnumerable<TData>> RetrieveAsync(INonTerminalDescriptor descriptor);
        
        Task<IEnumerable<TData>> RetrieveAsync(INonTerminalDescriptor descriptor, IDataBuilder<TData> builder);
    }
}
