using System.Threading.Tasks;

namespace DAA.StateManagement.Interfaces
{
    public interface IRetrievalContext
    {
        Task CompleteReconstitutionAsync();
    }
}