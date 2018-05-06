using System.Collections.Generic;

namespace DAA.StateManagement
{
    public interface IDataPool<TData> where TData : IData
    {
        void Store(IDescriptor descriptor, ICollection<TData> data);
    }
}
