using DAA.StateManagement.Interfaces;

namespace DAA.StateManagement
{
    public class InstanceRetrievalContext<TData> : RetrievalContext, IInstanceRetrievalContext<TData>
        where TData : IData
    {
        public InstanceRetrievalContext(TData data)
        {
            Data = data;
        }

        public TData Data { get; }
    }
}
