using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAA.StateManagement.Interfaces
{
    public interface ICollectionsManager<TData>
        where TData : IData
    {
        Task FillCollectionAsync(ICollection<TData> collection, INonTerminalDescriptor descriptor);
        bool IsCollectionRegistered(ICollection<TData> collection);
        void DropCollection(ICollection<TData> collection);
    }
}
