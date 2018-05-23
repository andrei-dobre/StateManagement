using DAA.StateManagement.Interfaces;

namespace DAA.StateManagement.DataManagement
{
    public class DataStore<TData> : DataInformationStore<ITerminalDescriptor, TData>
        where TData : IData
    {
        private IDataManipulator<TData> dataManipulator;

        protected IDataManipulator<TData> DataManipulator { get => dataManipulator; }


        public DataStore(IDataManipulator<TData> dataManipulator)
        {
            this.dataManipulator = dataManipulator;
        }


        public override void Update(ITerminalDescriptor descriptor, TData data)
        {
            var containedInstance = this.Retrieve(descriptor);

            this.DataManipulator.Update(containedInstance, data);
        }
    }
}
