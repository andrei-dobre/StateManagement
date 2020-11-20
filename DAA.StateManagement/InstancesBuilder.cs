using System.Collections.Generic;
using System.Threading.Tasks;
using DAA.StateManagement.Interfaces;

namespace DAA.StateManagement
{
    public class InstancesBuilder<TData> : IInstancesBuilder<TData>
        where TData : IData
    {
        public InstancesBuilder(IStateManagementEventsAggregator<TData> eventsAggregator)
        {
            EventsAggregator = eventsAggregator;
            EventsAggregator.InstanceChangedEvent += WhenInstanceChanged;

            BuildersByDescriptor = new Dictionary<ITerminalDescriptor, ICollection<IDataBuilder<TData>>>();
        }

        private IDictionary<ITerminalDescriptor, ICollection<IDataBuilder<TData>>> BuildersByDescriptor { get; }

        protected IStateManagementEventsAggregator<TData> EventsAggregator { get; }

        public virtual async Task BuildInstanceAsync(ITerminalDescriptor descriptor, TData instance)
        {
            if (BuildersByDescriptor.ContainsKey(descriptor))
            {
                foreach (var builder in BuildersByDescriptor[descriptor])
                {
                    await builder.DoWorkAsync(instance);
                }
            }
        }

        public void EnqueueBuilderForInstance(ITerminalDescriptor descriptor, IDataBuilder<TData> builder)
        {
            if (!BuildersByDescriptor.ContainsKey(descriptor))
            {
                BuildersByDescriptor[descriptor] = new List<IDataBuilder<TData>>();
            }

            BuildersByDescriptor[descriptor].Add(builder);
        }

        public void WhenInstanceChanged(object sender, InstanceChangedEventArgs<TData> args)
        {
            BuildInstanceAsync(args.Descriptor, args.Instance).ContinueWith(_ => { });
        }
    }
}
