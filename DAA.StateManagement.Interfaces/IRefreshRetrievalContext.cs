using System.Collections.Generic;

namespace DAA.StateManagement.Interfaces
{
    public interface IRefreshRetrievalContext<TData>
        where TData : IData
    {
        IInstanceRetrievalContext<TData> GetResult(ITerminalDescriptor descriptor);

        IEnumerable<ITerminalDescriptor> GetResult(INonTerminalDescriptor descriptor);
    }
}