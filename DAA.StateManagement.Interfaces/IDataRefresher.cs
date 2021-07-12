using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAA.StateManagement.Interfaces
{
    public interface IDataRefresher<TData>
        where TData: IData
    {
        Task RefreshAsync(IEnumerable<IDescriptor> descriptors);
        
        Task RefreshAsync(IDescriptor descriptor);
    }
}
