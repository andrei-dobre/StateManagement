using DAA.StateManagement.Interfaces;

namespace DAA.StateManagement
{
    public class StateManagementService<TData> : IStateManagementService<TData>
        where TData: IData
    { }
}
