using System.Threading.Tasks;

namespace DAA.StateManagement.Interfaces
{
    public interface IInstancesBuilder<TData> where TData : IData
    {
        Task BuildInstanceAsync(ITerminalDescriptor descriptor, TData instance);
        
        void EnqueueBuilderForInstance(ITerminalDescriptor descriptor, IDataBuilder<TData> builder);
    }
}