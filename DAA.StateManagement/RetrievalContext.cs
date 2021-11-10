using System;
using System.Threading.Tasks;
using DAA.StateManagement.Interfaces;

namespace DAA.StateManagement
{
    public class RetrievalContext : IRetrievalContext
    {
        public event EventHandler DataAddedEvent;
        
        public event EventHandler DataReconstitutedEvent;

        public void PublishDataAddedEvent()
        {
            DataAddedEvent?.Invoke(this, EventArgs.Empty);
        }

        public void PublishDataReconstitutedEvent()
        {
            DataReconstitutedEvent?.Invoke(this, EventArgs.Empty);
        }

        public virtual Task CompleteReconstitutionAsync()
        {
            return Task.CompletedTask;
        }
    }
}