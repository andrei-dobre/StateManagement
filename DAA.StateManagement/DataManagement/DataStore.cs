using DAA.StateManagement.Interfaces;

namespace DAA.StateManagement.DataManagement
{
    public class DataStore<TData> : Store<ITerminalDescriptor, TData>
        where TData : IData
    {
        public DataStore()
        { }
        
        public override void Update(ITerminalDescriptor descriptor, TData data)
        {
            // TODO: Implement me using the data manipulator.
        }
    }
}
