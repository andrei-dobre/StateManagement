using System.Collections.Generic;

namespace DAA.StateManagement.Interfaces
{
    public interface IDataPool<TData> where TData : IData
    {
        void Store(IDescriptor descriptor, IEnumerable<TData> data);
    }
}
