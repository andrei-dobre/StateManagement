using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAA.Helpers;
using DAA.StateManagement.Interfaces;

namespace DAA.StateManagement
{
    public class CollectionsManager<TData> : ICollectionsManager<TData>
        where TData : IData
    {
        private IDataPool<TData> DataPool { get; }
        private IStateEventsAggregator<TData> StateEventsAggregator { get; }

        private IDictionary<INonTerminalDescriptor, ICollection<ICollection<TData>>> CollectionsByDescriptor { get; }
        private IDictionary<ICollection<TData>, INonTerminalDescriptor> DescriptorByCollection { get; }


        public CollectionsManager(IDataPool<TData> dataPool, IStateEventsAggregator<TData> stateEventsAggregator)
        {
            DataPool = dataPool;
            StateEventsAggregator = stateEventsAggregator;

            CollectionsByDescriptor = new Dictionary<INonTerminalDescriptor, ICollection<ICollection<TData>>>();
            DescriptorByCollection = new Dictionary<ICollection<TData>, INonTerminalDescriptor>();

            StateEventsAggregator.CompositionChangedEvent += WhenCompositionChanged;
        }


        public async Task FillCollectionAsync(ICollection<TData> collection, INonTerminalDescriptor descriptor)
        {
            RegisterCollection(collection, descriptor);
            FillCollectionWithData(collection, descriptor);
        }

        public virtual void DropCollection(ICollection<TData> collection)
        {
            if (CollectionIsRegistered(collection))
            {
                ClearCollection(collection);
                DropCollection(collection, GetDescriptor(collection));
            }
        }

        public virtual void WhenCompositionChanged(object sender, INonTerminalDescriptor descriptor)
        {
            FindAffectedCollections(descriptor).ForEach(UpdateCollection);
        }

        public virtual void RegisterCollection(ICollection<TData> collection, INonTerminalDescriptor descriptor)
        {
            DropCollection(collection);

            GetCollectionsForDescriptor(descriptor).Add(collection);
            DescriptorByCollection.Add(collection, descriptor);
        }

        public virtual void FillCollectionWithData(ICollection<TData> collection, INonTerminalDescriptor descriptor)
        {
            ClearCollection(collection);
            DataPool.Retrieve(descriptor).ForEach(collection.Add);
        }

        public virtual IEnumerable<ICollection<TData>> FindAffectedCollections(INonTerminalDescriptor changedDescriptor)
        {
            return FindIntersectingDescriptors(changedDescriptor).SelectMany(_ => CollectionsByDescriptor[_]).ToArray();
        }

        public virtual IEnumerable<INonTerminalDescriptor> FindIntersectingDescriptors(INonTerminalDescriptor changedDescriptor)
        {
            return CollectionsByDescriptor.Keys.Where(_ => _.Intersects(changedDescriptor));
        }

        public virtual void UpdateCollection(ICollection<TData> collection)
        {
            var descriptor = GetDescriptor(collection);
            var data = DataPool.Retrieve(descriptor);

            collection.Update(data);
        }

        public virtual void DropCollection(ICollection<TData> collection, INonTerminalDescriptor descriptor)
        {
            DescriptorByCollection.Remove(collection);
            CollectionsByDescriptor[descriptor].Remove(collection);
        }

        public virtual void ClearCollection(ICollection<TData> collection)
        {
            collection.RemoveClear();
        }

        public virtual INonTerminalDescriptor GetDescriptor(ICollection<TData> collection)
        {
            return DescriptorByCollection[collection];
        }

        public virtual bool CollectionIsRegistered(ICollection<TData> collection)
        {
            return DescriptorByCollection.ContainsKey(collection);
        }

        public virtual ICollection<ICollection<TData>> GetCollectionsForDescriptor(INonTerminalDescriptor descriptor)
        {
            if (!CollectionsByDescriptor.ContainsKey(descriptor))
            {
                CollectionsByDescriptor[descriptor] = new List<ICollection<TData>>();
            }

            return CollectionsByDescriptor[descriptor];
        }
    }
}
