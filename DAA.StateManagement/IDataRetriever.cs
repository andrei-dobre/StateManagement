using System.Collections.Generic;

namespace DAA.StateManagement
{
    public interface IDataRetriever<TData> where TData : IData
    {
        TData Fetch(ITerminalDescriptor descriptor);

        IEnumerable<TData> Fetch(INonTerminalDescriptor descriptor);

        IEnumerable<object> FetchComposition(INonTerminalDescriptor descriptor);
    }
}
