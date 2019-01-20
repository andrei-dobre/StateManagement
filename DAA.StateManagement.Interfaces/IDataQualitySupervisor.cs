using System.Threading.Tasks;

namespace DAA.StateManagement.Interfaces
{
    public interface IDataQualitySupervisor<TData>
        where TData : IData
    {
        Task AcknowledgeStaleDataAsync(IDescriptor descriptor);
    }
}
