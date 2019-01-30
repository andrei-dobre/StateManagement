using System;
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
        private IStateManagementEventsAggregator<TData> EventsAggregator { get; }

        private IDictionary<INonTerminalDescriptor, ICollection<ICollection<TData>>> CollectionsByDescriptor { get; }
        private IDictionary<ICollection<TData>, INonTerminalDescriptor> DescriptorByCollection { get; }
        private IDictionary<ICollection<TData>, IDataBuilder<TData>> BuilderByCollection { get; }


        public CollectionsManager(IDataPool<TData> dataPool, IStateManagementEventsAggregator<TData> eventsAggregator)
        {
            DataPool = dataPool;
            EventsAggregator = eventsAggregator;

            CollectionsByDescriptor = new Dictionary<INonTerminalDescriptor, ICollection<ICollection<TData>>>();
            DescriptorByCollection = new Dictionary<ICollection<TData>, INonTerminalDescriptor>();
            BuilderByCollection = new Dictionary<ICollection<TData>, IDataBuilder<TData>>();

            EventsAggregator.CompositionChangedEvent += WhenCompositionChanged;
        }


        public async Task FillCollectionAsync(IFillCollectionArgs<TData> args)
        {
            RegisterCollection(args.Collection, args.Descriptor, args.Builder);
            FillCollectionWithData(args.Collection, args.Descriptor);

            await BuildCollectionAsync(args.Collection);
        }

        public async Task ChangeBuilderAsync(ICollection<TData> collection, IDataBuilder<TData> builder)
        {
            if (!IsCollectionRegistered(collection))
            {
                throw new InvalidOperationException("Cannot change the builder associated with a collection that has not been registered");
            }

            BuilderByCollection[collection] = builder;
            await BuildCollectionAsync(collection);
        }

        public virtual void DropCollection(ICollection<TData> collection)
        {
            if (IsCollectionRegistered(collection))
            {
                ClearCollection(collection);
                DropCollection(collection, GetDescriptor(collection));
            }
        }

        public virtual bool IsCollectionRegistered(ICollection<TData> collection)
        {
            return DescriptorByCollection.ContainsKey(collection);
        }

        public virtual bool IsCollectionRegisteredWithDescriptor(ICollection<TData> collection, INonTerminalDescriptor descriptor)
        {
            return CollectionsByDescriptor.ContainsKey(descriptor) 
                   && CollectionsByDescriptor[descriptor].Contains(collection);
        }

        public virtual async void WhenCompositionChanged(object sender, INonTerminalDescriptor descriptor)
        {
            await Task.WhenAll(FindAffectedCollections(descriptor).Select(UpdateCollectionAsync).ToArray());
        }

        public virtual void RegisterCollection(ICollection<TData> collection, INonTerminalDescriptor descriptor, IDataBuilder<TData> builder)
        {
            DropCollection(collection);

            GetCollectionsForDescriptor(descriptor).Add(collection);
            DescriptorByCollection.Add(collection, descriptor);
            BuilderByCollection.Add(collection, builder);
        }

        public virtual void FillCollectionWithData(ICollection<TData> collection, INonTerminalDescriptor descriptor)
        {
            ClearCollection(collection);
            AppendToCollection(DataPool.Retrieve(descriptor), collection);
        }

        public virtual IEnumerable<ICollection<TData>> FindAffectedCollections(INonTerminalDescriptor changedDescriptor)
        {
            return FindIntersectingDescriptors(changedDescriptor).SelectMany(_ => CollectionsByDescriptor[_]).ToArray();
        }

        public virtual IEnumerable<INonTerminalDescriptor> FindIntersectingDescriptors(INonTerminalDescriptor changedDescriptor)
        {
            return CollectionsByDescriptor.Keys.Where(_ => _.Intersects(changedDescriptor));
        }

        public virtual async Task UpdateCollectionAsync(ICollection<TData> collection)
        {
            var descriptor = GetDescriptor(collection);
            var data = DataPool.Retrieve(descriptor);

            UpdateCollectionContent(collection, data);

            await BuildCollectionAsync(collection);
        }

        public virtual void DropCollection(ICollection<TData> collection, INonTerminalDescriptor descriptor)
        {
            DescriptorByCollection.Remove(collection);
            CollectionsByDescriptor[descriptor].Remove(collection);
            BuilderByCollection.Remove(collection);
        }

        public virtual void ClearCollection(ICollection<TData> collection)
        {
            PerformCollectionMutations(() => collection.RemoveClear());
        }

        public void UpdateCollectionContent(ICollection<TData> collection, IEnumerable<TData> data)
        {
            PerformCollectionMutations(() => collection.Update(data));
        }

        public void AppendToCollection(IEnumerable<TData> data, ICollection<TData> collection)
        {
            PerformCollectionMutations(() => data.ForEach(collection.Add));
        }

        public virtual async Task BuildCollectionAsync(ICollection<TData> collection)
        {
            var builder = BuilderByCollection[collection];

            foreach (var item in collection)
            {
                await builder.DoWorkAsync(item);
            }
        }

        public virtual INonTerminalDescriptor GetDescriptor(ICollection<TData> collection)
        {
            return DescriptorByCollection[collection];
        }

        public virtual ICollection<ICollection<TData>> GetCollectionsForDescriptor(INonTerminalDescriptor descriptor)
        {
            if (!CollectionsByDescriptor.ContainsKey(descriptor))
            {
                CollectionsByDescriptor[descriptor] = new List<ICollection<TData>>();
            }

            return CollectionsByDescriptor[descriptor];
        }

        public virtual void PerformCollectionMutations(Action fn)
        {
            fn();
        }
    }
}
