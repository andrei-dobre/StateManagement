using System.Collections.Generic;

namespace DAA.StateManagement.Interfaces
{
    public interface ITerminalDescriptorsFactory<TData>
        where TData : IData
    {
        ITerminalDescriptor Create(TData data);

        IEnumerable<ITerminalDescriptor> Create(IEnumerable<TData> data);
    }
}
