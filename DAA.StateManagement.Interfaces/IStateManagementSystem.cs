namespace DAA.StateManagement.Interfaces
{
    public interface IStateManagementSystem<TData>
        where TData : IData
    {
        IDataQualitySupervisor<TData> QualitySupervisor { get; }
        IDataRepository<TData> Repository { get; }
        IStateManagementEventsAggregator<TData> EventsAggregator { get; }
    }
}
