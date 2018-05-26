using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAA.StateManagement.Interfaces
{
    public interface IStateManagementService<TData>
        where TData: IData
    {
        Task RefreshIntersectingDataAsync(IDescriptor descriptor);

        Task RefreshDataAsync(IEnumerable<IDescriptor> descriptors);

        Task RefreshDataAsync(IDescriptor descriptor);

        Task RefreshDataAsync(ITerminalDescriptor descriptor);

        Task RefreshDataAsync(INonTerminalDescriptor descriptor);
    }
}
