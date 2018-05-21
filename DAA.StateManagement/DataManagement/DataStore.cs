using System.Collections.Generic;

using DAA.StateManagement.Interfaces;

namespace DAA.StateManagement.DataManagement
{
    public class DataStore<TData> : DataInformationStore<ITerminalDescriptor, TData>
        where TData : IData
    {
        public DataStore()
        { }

        public override void Update(ITerminalDescriptor descriptor, TData data)
        {
            // TODO: Implement using the data manipulator.
        }
    }
}
