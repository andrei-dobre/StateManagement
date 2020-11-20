using System.Threading.Tasks;
using DAA.StateManagement.Interfaces;

namespace DAA.StateManagement
{
    public class RetrievalContext : IRetrievalContext
    {
        public virtual Task CompleteReconstitutionAsync()
        {
            return Task.CompletedTask;
        }
    }
}