using System.Collections.Generic;
using DAA.StateManagement.Interfaces;

namespace DAA.StateManagement
{
    public class CollectionRetrievalContext<TData> : RetrievalContext, ICollectionRetrievalContext<TData>
        where TData : IData
    {
        public CollectionRetrievalContext(IEnumerable<TData> data)
        {
            Data = data;
        }

        public IEnumerable<TData> Data { get; }
    }
}