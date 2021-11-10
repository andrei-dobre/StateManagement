using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAA.StateManagement.Interfaces
{
    public interface IDataRefresher<TData>
        where TData: IData
    {
        Task RefreshAsync(IDescriptor descriptor);
        
        Task RefreshAsync(IEnumerable<IDescriptor> descriptors);
    }
}
