using System.Threading.Tasks;

namespace DAA.StateManagement.Interfaces
{
    public interface IDataBuilder<TData>
        where TData : IData
    {
        Task DoWorkAsync(TData data);
    }
}
