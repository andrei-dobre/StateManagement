using System.Collections.Generic;
using System.Threading.Tasks;
using DAA.StateManagement.Interfaces;

namespace DAA.StateManagement
{
    public class DataRepository<TData> : IDataRepository<TData>
        where TData : IData
    {
        private readonly DescriptorSemaphore _semaphore;
        
        public DataRepository(IDataRetriever<TData> dataRetriever, IDataPool<TData> dataPool, 
            ICollectionsManager<TData> collectionsManager, IInstancesBuilder<TData> instancesBuilder, 
            IStateManagementEventsAggregator<TData> eventsAggregator)
        {
            _semaphore = new DescriptorSemaphore();
            
            DataRetriever = dataRetriever;
            DataPool = dataPool;
            CollectionsManager = collectionsManager;
            InstancesBuilder = instancesBuilder;
            EventsAggregator = eventsAggregator;
            
            EventsAggregator.TerminalDataAvailableEvent += OnTerminalDataAvailableEvent;
            EventsAggregator.NonTerminalDataAvailableEvent += OnNonTerminalDataAvailableEvent;
        }

        public virtual IDataRetriever<TData> DataRetriever { get; }

        public virtual IDataPool<TData> DataPool { get; }

        public virtual ICollectionsManager<TData> CollectionsManager { get; }

        public virtual IInstancesBuilder<TData> InstancesBuilder { get; }

        public virtual IStateManagementEventsAggregator<TData> EventsAggregator { get; }

        public virtual async Task<TData> RetrieveAsync(ITerminalDescriptor descriptor)
        {
            await Acquire(descriptor);
            
            return DataPool.Retrieve(descriptor);
        }

        public virtual async Task<IEnumerable<TData>> RetrieveAsync(INonTerminalDescriptor descriptor)
        {
            await Acquire(descriptor);
            
            return DataPool.Retrieve(descriptor);
        }

        public async Task FillCollectionAsync(IFillCollectionArgs<TData> args)
        {
            await Acquire(args.Descriptor);
            
            await CollectionsManager.FillCollectionAsync(args);
        }

        public virtual async Task<TData> RetrieveAsync(ITerminalDescriptor descriptor, IDataBuilder<TData> builder)
        {
            var instance = await RetrieveAsync(descriptor);

            InstancesBuilder.EnqueueBuilderForInstance(descriptor, builder);
            await builder.DoWorkAsync(instance);

            return instance;
        }

        public async Task<IEnumerable<TData>> RetrieveAsync(INonTerminalDescriptor descriptor, IDataBuilder<TData> builder)
        {
            var data = await RetrieveAsync(descriptor);

            foreach (var instance in data)
            {
                await builder.DoWorkAsync(instance);
            }

            return data;
        }

        public async Task ChangeBuilderAsync(ICollection<TData> collection, IDataBuilder<TData> builder)
        {
            await CollectionsManager.ChangeBuilderAsync(collection, builder);
        }

        public bool IsCollectionRegistered(ICollection<TData> collection)
        {
            return CollectionsManager.IsCollectionRegistered(collection);
        }

        public bool IsCollectionRegisteredWithDescriptor(ICollection<TData> collection, INonTerminalDescriptor descriptor)
        {
            return CollectionsManager.IsCollectionRegisteredWithDescriptor(collection, descriptor);
        }

        public void DropCollection(ICollection<TData> collection)
        {
            CollectionsManager.DropCollection(collection);
        }

        public virtual async Task Acquire(ITerminalDescriptor descriptor)
        {
            var semaphore = await _semaphore.WaitAsync(descriptor);

            try
            {
                if (DataPool.Contains(descriptor) == false)
                {
                    await DataPool.SaveAsync(descriptor, await DataRetriever.RetrieveAsync(descriptor), semaphore.Release);
                }
            }
            finally
            {
                semaphore.Release();
            }
        }

        public virtual async Task Acquire(INonTerminalDescriptor descriptor)
        {
            var semaphore = await _semaphore.WaitAsync(descriptor);

            try
            {
                if (DataPool.Contains(descriptor) == false)
                {
                    await DataPool.SaveAsync(descriptor,
                        await DataRetriever.RetrieveAsync(descriptor), semaphore.Release);
                }
            }
            finally
            {
                semaphore.Release();
            }
        }

        private async void OnTerminalDataAvailableEvent(object sender, TerminalDataAvailableEventArgs<TData> e)
        {
            var semaphore = await _semaphore.WaitAsync(e.Descriptor);

            try
            {
                await DataPool.SaveAsync(e.Descriptor, e.RetrievalContext, semaphore.Release);
            }
            finally
            {
                semaphore.Release();
            }
        }

        private async void OnNonTerminalDataAvailableEvent(object sender, NonTerminalDataAvailableEventArgs<TData> e)
        {
            var semaphore = await _semaphore.WaitAsync(e.Descriptor);

            try
            {
                await DataPool.SaveAsync(e.Descriptor, e.RetrievalContext, semaphore.Release);
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}
