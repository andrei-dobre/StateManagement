using System.Collections.Generic;

namespace DAA.StateManagement.Interfaces
{
    public interface ICollectionRetrievalContext<TData> : IRetrievalContext
        where TData : IData
    {
        IEnumerable<TData> Data { get; }
    }
}