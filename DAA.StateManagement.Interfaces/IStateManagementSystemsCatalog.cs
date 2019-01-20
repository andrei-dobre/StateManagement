namespace DAA.StateManagement.Interfaces
{
    public interface IStateManagementSystemsCatalog
    {
        void Register<TData>(IStateManagementSystem<TData> stateManagementSystem) where TData : IData;
        IStateManagementSystem<TData> Retrieve<TData>() where TData : IData;
    }
}
