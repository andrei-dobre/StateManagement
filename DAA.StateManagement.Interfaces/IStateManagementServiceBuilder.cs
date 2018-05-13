namespace DAA.StateManagement.Interfaces
{
    public interface IStateManagementServiceBuilder<TData> : IStateManagementServiceBuildingInterface
        where TData: IData
    {
        IStateManagementService<TData> ExtractResult();
    }
}
