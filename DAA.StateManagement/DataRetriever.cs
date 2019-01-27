using System.Collections.Generic;
using System.Threading.Tasks;
using DAA.StateManagement.Interfaces;

namespace DAA.StateManagement
{
    public abstract class DataRetriever<TData> : IDataRetriever<TData>
        where TData : IData
    {
        protected ITerminalDescriptorsFactory<TData> TerminalDescriptorsFactory { get; }


        public DataRetriever(ITerminalDescriptorsFactory<TData> terminalDescriptorsFactory)
        {
            TerminalDescriptorsFactory = terminalDescriptorsFactory;
        }


        public abstract Task<TData> RetrieveAsync(ITerminalDescriptor descriptor);
        public abstract Task<IEnumerable<TData>> RetrieveAsync(IEnumerable<ITerminalDescriptor> descriptors);
        public abstract Task<IEnumerable<TData>> RetrieveAsync(INonTerminalDescriptor descriptor);
        public abstract Task<IEnumerable<ITerminalDescriptor>> RetrieveCompositionAsync(INonTerminalDescriptor descriptor);
    }
}
