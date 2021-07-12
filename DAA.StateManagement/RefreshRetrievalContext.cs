using System;
using System.Collections.Generic;
using DAA.StateManagement.Interfaces;

namespace DAA.StateManagement
{
    public class RefreshRetrievalContext<TData> : IRefreshRetrievalContext<TData>
        where TData : IData
    {
        private readonly IDictionary<ITerminalDescriptor, IInstanceRetrievalContext<TData>> _resultsForTerminal;
        private readonly IDictionary<INonTerminalDescriptor, IEnumerable<ITerminalDescriptor>> _resultsForNonTerminal;

        public RefreshRetrievalContext()
        {
            _resultsForTerminal = new Dictionary<ITerminalDescriptor, IInstanceRetrievalContext<TData>>();
            _resultsForNonTerminal = new Dictionary<INonTerminalDescriptor, IEnumerable<ITerminalDescriptor>>();
        }
        
        public RefreshRetrievalContext<TData> SetResult(ITerminalDescriptor descriptor, IInstanceRetrievalContext<TData> instanceRetrievalContext)
        {
            _resultsForTerminal[descriptor] = instanceRetrievalContext;
            return this;
        }

        public RefreshRetrievalContext<TData> SetResult(INonTerminalDescriptor descriptor, IEnumerable<ITerminalDescriptor> composition)
        {
            _resultsForNonTerminal[descriptor] = composition;
            return this;
        }
        
        public IInstanceRetrievalContext<TData> GetResult(ITerminalDescriptor descriptor)
        {
            if (_resultsForTerminal.ContainsKey(descriptor))
            {
                return _resultsForTerminal[descriptor];
            }

            throw new ArgumentException("Could not find a result for the given descriptor");
        }

        public IEnumerable<ITerminalDescriptor> GetResult(INonTerminalDescriptor descriptor)
        {
            if (_resultsForNonTerminal.ContainsKey(descriptor))
            {
                return _resultsForNonTerminal[descriptor];
            }

            throw new ArgumentException("Could not find a result for the given descriptor");
        }
    }
}