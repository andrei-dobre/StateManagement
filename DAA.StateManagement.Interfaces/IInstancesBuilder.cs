using System.Threading.Tasks;
using DAA.StateManagement.Interfaces;

namespace DAA.StateManagement
{
    public interface IInstancesBuilder<TData> where TData : IData
    {
        Task BuildInstanceAsync(ITerminalDescriptor descriptor, TData instance);
        void EnqueueBuilderForInstance(ITerminalDescriptor descriptor, IDataBuilder<TData> builder);
    }
}