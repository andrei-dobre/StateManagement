using System.Collections.Generic;
using DAA.StateManagement.Interfaces;

namespace DAA.StateManagement
{
    public class FillCollectionArgs<TData> : IFillCollectionArgs<TData>
        where TData : IData
    {
        public INonTerminalDescriptor Descriptor { get; }
        public ICollection<TData> Collection { get; }
        public IDataBuilder<TData> Builder { get; }


        public FillCollectionArgs(ICollection<TData> collection, INonTerminalDescriptor descriptor, IDataBuilder<TData> builder)
        {
            Collection = collection;
            Descriptor = descriptor;
            Builder = builder;
        }

        public FillCollectionArgs(ICollection<TData> collection, INonTerminalDescriptor descriptor)
            : this(collection, descriptor, new NullDataBuilder<TData>())
        { }
    }
}
