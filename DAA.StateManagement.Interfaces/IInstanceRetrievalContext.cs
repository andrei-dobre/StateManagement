namespace DAA.StateManagement.Interfaces
{
    public interface IInstanceRetrievalContext<TData> : IRetrievalContext
        where TData : IData
    {
        TData Data { get; }
    }
}