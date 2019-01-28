using System.Threading.Tasks;
using DAA.StateManagement.Interfaces;

namespace DAA.StateManagement
{
    public class NullDataBuilder<TData> : IDataBuilder<TData>
        where TData : IData
    {
        public Task DoWorkAsync(TData data)
        {
            return Task.FromResult(0);
        }
    }
}
