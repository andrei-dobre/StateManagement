using System.Collections.Generic;

namespace DAA.StateManagement.Interfaces
{
    public interface IFillCollectionArgs<TData> 
        where TData : IData
    {
        INonTerminalDescriptor Descriptor { get; }
        
        IDataBuilder<TData> Builder { get; }
        
        ICollection<TData> Collection { get; }
    }
}