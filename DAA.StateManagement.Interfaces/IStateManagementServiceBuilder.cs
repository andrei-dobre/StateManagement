namespace DAA.StateManagement.Interfaces
{
    public interface IStateManagementServiceBuilder<TData> : IStateManagementServiceBuildingOperations
        where TData: IData
    {
        IStateManagementService<TData> ExtractResult();
    }
}
