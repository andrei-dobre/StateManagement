using System;

namespace DAA.StateManagement.Interfaces
{
    public class InstanceChangedEventArgs<TData> : EventArgs
    {
        public ITerminalDescriptor Descriptor { get; }
        public TData Instance { get; }


        public InstanceChangedEventArgs(ITerminalDescriptor descriptor, TData instance)
        {
            Descriptor = descriptor;
            Instance = instance;
        }
    }
}
