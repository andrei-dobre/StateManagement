namespace DAA.StateManagement.Interfaces
{
    public interface IStateManagementSystemBuilder<TData> : IStateManagementSystemBuildingOperations
        where TData: IData
    {
        IDataRefresher<TData> ExtractResult();
    }
}
