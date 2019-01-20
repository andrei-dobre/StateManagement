using System.Collections.Generic;

namespace DAA.StateManagement.Interfaces
{
    public interface IDataPool<TData> 
        where TData : IData
    {
        bool Contains(ITerminalDescriptor descriptor);

        bool Contains(INonTerminalDescriptor descriptor);


        TData Retrieve(ITerminalDescriptor descriptor);

        IEnumerable<TData> Retrieve(INonTerminalDescriptor descriptor);


        IEnumerable<ITerminalDescriptor> UpdateCompositionAndProvideAdditions(INonTerminalDescriptor descriptor, IEnumerable<ITerminalDescriptor> composition);


        void Save(ITerminalDescriptor descriptor, TData data);

        void Save(INonTerminalDescriptor descriptor, IEnumerable<TData> data);

        void Save(IEnumerable<TData> data);


        IEnumerable<IDescriptor> FindIntersectingDescriptors(IDescriptor descriptor);
    }
}
