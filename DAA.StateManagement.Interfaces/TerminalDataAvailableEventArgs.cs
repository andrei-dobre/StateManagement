using System;

namespace DAA.StateManagement.Interfaces
{
    public class TerminalDataAvailableEventArgs<TData> : EventArgs
        where TData : IData
    {
        public TerminalDataAvailableEventArgs(ITerminalDescriptor descriptor, 
            IInstanceRetrievalContext<TData> retrievalContext)
        {
            Descriptor = descriptor;
            RetrievalContext = retrievalContext;
        }

        public ITerminalDescriptor Descriptor { get; }
        
        public IInstanceRetrievalContext<TData> RetrievalContext { get; }
    }
}