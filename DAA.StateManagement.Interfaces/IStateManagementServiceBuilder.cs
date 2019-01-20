namespace DAA.StateManagement.Interfaces
{
    public interface IStateManagementServiceBuilder<TData> : IStateManagementServiceBuildingOperations
        where TData: IData
    {
        IDataRefresher<TData> ExtractResult();
    }
}
