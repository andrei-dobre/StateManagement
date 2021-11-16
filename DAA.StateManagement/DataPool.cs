using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAA.StateManagement.Interfaces;
using DAA.StateManagement.Stores;

namespace DAA.StateManagement
{
    public class DataPool<TData> : IDataPool<TData>
        where TData : IData
    {
        public DataPool(ITerminalDescriptorsFactory<TData> terminalDescriptorsFactory, IDataManipulator<TData> dataManipulator)
        {
            TerminalDescriptorsFactory = terminalDescriptorsFactory;

            Data = new DataStore<TData>(dataManipulator);
            Compositions = new CompositionsStore();
        }

        protected virtual DataStore<TData> Data { get;  }

        protected virtual CompositionsStore Compositions { get; }

        protected virtual ITerminalDescriptorsFactory<TData> TerminalDescriptorsFactory { get; }

        public bool Contains(ITerminalDescriptor descriptor)
        {
            return Data.Contains(descriptor);
        }

        public bool Contains(INonTerminalDescriptor descriptor)
        {
            return Compositions.Contains(descriptor);
        }

        public virtual TData Retrieve(ITerminalDescriptor descriptor)
        {
            return Data.Retrieve(descriptor);
        }

        public virtual IEnumerable<TData> Retrieve(INonTerminalDescriptor descriptor)
        {
            return Compositions.Retrieve(descriptor).Select(Retrieve).ToArray();
        }

        public async Task SaveAsync(ITerminalDescriptor descriptor, IInstanceRetrievalContext<TData> retrievalContext)
        {
            await SaveAsync(descriptor, retrievalContext, doAfterDataAdded: null);
        }
        
        public async Task SaveAsync(ITerminalDescriptor descriptor, IInstanceRetrievalContext<TData> retrievalContext, Action doAfterDataAdded)
        {
            var isExistingInstance = !Data.Add(descriptor, retrievalContext.Data);
            
            doAfterDataAdded?.Invoke();
            
            retrievalContext.PublishDataAddedEvent();
            await retrievalContext.CompleteReconstitutionAsync();
            retrievalContext.PublishDataReconstitutedEvent();
            
            if (isExistingInstance)
            {
                Data.Update(descriptor, retrievalContext.Data);
            }
        }

        public async Task SaveAsync(INonTerminalDescriptor descriptor, ICollectionRetrievalContext<TData> retrievalContext)
        {
            await SaveAsync(descriptor, retrievalContext, doAfterDataAdded: null);
        }

        public async Task SaveAsync(INonTerminalDescriptor descriptor, ICollectionRetrievalContext<TData> retrievalContext, Action doAfterDataAdded)
        {
            Compositions.Save(descriptor, Describe(retrievalContext.Data));
            
            await SaveAsync(retrievalContext, () =>
            {
                Compositions.Save(descriptor, Describe(retrievalContext.Data));
                doAfterDataAdded?.Invoke();
            });
        }

        public async Task SaveAsync(ICollectionRetrievalContext<TData> retrievalContext)
        {
            await SaveAsync(retrievalContext, doAfterDataAdded: null);
        }

        public async Task SaveAsync(ICollectionRetrievalContext<TData> retrievalContext, Action doAfterDataAdded)
        {
            var existingInstancesByDescriptor = new Dictionary<ITerminalDescriptor, TData>();

            foreach (var data in retrievalContext.Data)
            {
                var terminalDescriptor = TerminalDescriptorsFactory.Create(data);
                
                var isExistingInstance = !Data.Add(terminalDescriptor, data);
                if (isExistingInstance)
                {
                    existingInstancesByDescriptor[terminalDescriptor] = data;
                }
            }

            doAfterDataAdded?.Invoke();
            
            retrievalContext.PublishDataAddedEvent();
            await retrievalContext.CompleteReconstitutionAsync();
            retrievalContext.PublishDataReconstitutedEvent();

            foreach (var existingInstanceByDescriptor in existingInstancesByDescriptor)
            {
                Data.Update(existingInstanceByDescriptor.Key, existingInstanceByDescriptor.Value);
            }
        }

        public IEnumerable<IDescriptor> FindIntersectingDescriptors(IDescriptor descriptor)
        {
            return RetrieveAllDescriptors().Where(_ => _.Intersects(descriptor)).ToArray();
        }

        public virtual IEnumerable<ITerminalDescriptor> UpdateCompositionAndProvideAdditions(INonTerminalDescriptor descriptor, IEnumerable<ITerminalDescriptor> composition)
        {
            return Compositions.UpdateAndProvideAdditions(descriptor, composition);
        }

        private IEnumerable<ITerminalDescriptor> Describe(IEnumerable<TData> data)
        {
            return data.Select(TerminalDescriptorsFactory.Create).ToArray();
        }

        protected virtual IEnumerable<IDescriptor> RetrieveAllDescriptors()
        {
            return Data
                .RetrieveDescriptors().Cast<IDescriptor>()
                .Concat(Compositions.RetrieveDescriptors());
        }
    }
}
