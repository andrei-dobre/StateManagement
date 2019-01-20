namespace DAA.StateManagement.Interfaces
{
    public interface IStateManagementSystemBuilder<TData> : IStateManagementSystemBuildingOperations
        where TData: IData
    {
        IStateManagementSystem<TData> ExtractResult();
    }
}
