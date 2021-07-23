using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        Task SaveAsync(ITerminalDescriptor descriptor, IInstanceRetrievalContext<TData> retrievalContext);
        
        Task SaveAsync(ITerminalDescriptor descriptor, IInstanceRetrievalContext<TData> retrievalContext, Action doAfterDataAdded);

        Task SaveAsync(INonTerminalDescriptor descriptor, ICollectionRetrievalContext<TData> retrievalContext);
        
        Task SaveAsync(INonTerminalDescriptor descriptor, ICollectionRetrievalContext<TData> retrievalContext, Action doAfterDataAdded);

        Task SaveAsync(ICollectionRetrievalContext<TData> retrievalContext);
        
        Task SaveAsync(ICollectionRetrievalContext<TData> retrievalContext, Action doAfterDataAdded);

        IEnumerable<IDescriptor> FindIntersectingDescriptors(IDescriptor descriptor);
    }
}
