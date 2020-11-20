using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAA.StateManagement.Interfaces
{
    public interface IStateManagement
    {
        Task<TData> RetrieveAsync<TData>(ITerminalDescriptor descriptor) where TData : IData;
        
        Task<TData> RetrieveAsync<TData>(ITerminalDescriptor descriptor, IDataBuilder<TData> builder) where TData : IData;
        
        Task<IEnumerable<TData>> RetrieveAsync<TData>(INonTerminalDescriptor descriptor) where TData : IData;
        
        Task<IEnumerable<TData>> RetrieveAsync<TData>(INonTerminalDescriptor descriptor, IDataBuilder<TData> builder) where TData : IData;
        
        Task FillCollectionAsync<TData>(ICollection<TData> collection, INonTerminalDescriptor descriptor) where TData : IData;
        
        Task FillCollectionAsync<TData>(ICollection<TData> collection, INonTerminalDescriptor descriptor, IDataBuilder<TData> builder) where TData : IData;

        Task ChangeBuilderAsync<TData>(ICollection<TData> collection, IDataBuilder<TData> builder) where TData : IData;
        
        bool IsCollectionRegistered<TData>(ICollection<TData> collection) where TData : IData;
        
        bool IsCollectionRegisteredWithDescriptor<TData>(ICollection<TData> collection, INonTerminalDescriptor descriptor) where TData : IData;
        
        void DropCollection<TData>(ICollection<TData> collection) where TData : IData;
        
        void Register<TData>(IStateManagementSystemBuilder<TData> builder) where TData : IData;
    }
}
