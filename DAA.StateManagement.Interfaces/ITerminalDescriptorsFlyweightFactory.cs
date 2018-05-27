using System.Collections.Generic;

namespace DAA.StateManagement.Interfaces
{
    public interface ITerminalDescriptorsFlyweightFactory
    {
        ITerminalDescriptor Create(object intrinsicState);

        ITerminalDescriptor Create(IData data);


        IEnumerable<ITerminalDescriptor> Create(IEnumerable<object> intrinsicStates);

        IEnumerable<ITerminalDescriptor> Create(IEnumerable<IData> data);
    }
}
