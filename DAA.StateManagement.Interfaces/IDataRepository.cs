namespace DAA.StateManagement.Interfaces
{
    public interface IDataRepository<TData> : ICollectionsManager<TData>
        where TData : IData
    { }
}
