using System.Collections.Generic;

using DAA.StateManagement.Interfaces;

namespace DAA.StateManagement.DataManagement
{
    public abstract class DataInformationStore<TDescriptor, TInformation> : Store<TDescriptor, TInformation>
        where TDescriptor : IDescriptor
    {
        public virtual IEnumerable<TDescriptor> RetrieveDescriptors()
        {
            return this.RetrieveKeys();
        }
    }
}
