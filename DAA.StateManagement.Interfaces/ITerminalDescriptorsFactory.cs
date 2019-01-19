using System.Collections.Generic;

namespace DAA.StateManagement.Interfaces
{
    public interface ITerminalDescriptorsFactory
    {
        ITerminalDescriptor Create(IData data);

        IEnumerable<ITerminalDescriptor> Create(IEnumerable<IData> data);
    }
}
