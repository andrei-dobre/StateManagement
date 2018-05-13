using System.Collections.Generic;

namespace DAA.StateManagement.Interfaces
{
    public interface ITerminalDescriptorsFlyweightFactory
    {
        ITerminalDescriptor Create(object intrinsicState);
        IEnumerable<ITerminalDescriptor> Create(IEnumerable<object> intrinsicStates);
    }
}
