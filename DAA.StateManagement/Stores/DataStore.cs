using DAA.StateManagement.Interfaces;

namespace DAA.StateManagement.Stores
{
    public class DataStore<TData> : StateManagementStore<ITerminalDescriptor, TData>
        where TData : IData
    {
        protected IDataManipulator<TData> DataManipulator { get; }


        public DataStore(IDataManipulator<TData> dataManipulator)
        {
            DataManipulator = dataManipulator;
        }


        public override void Update(ITerminalDescriptor descriptor, TData data)
        {
            var containedInstance = Retrieve(descriptor);

            DataManipulator.Update(containedInstance, data);
        }
    }
}
