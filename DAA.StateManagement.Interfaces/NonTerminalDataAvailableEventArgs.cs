using System;

namespace DAA.StateManagement.Interfaces
{
    public class NonTerminalDataAvailableEventArgs<TData> : EventArgs
        where TData : IData
    {
        public NonTerminalDataAvailableEventArgs(INonTerminalDescriptor descriptor, 
            ICollectionRetrievalContext<TData> retrievalContext)
        {
            Descriptor = descriptor;
            RetrievalContext = retrievalContext;
        }

        public INonTerminalDescriptor Descriptor { get; }
        
        public ICollectionRetrievalContext<TData> RetrievalContext { get; }
    }
}