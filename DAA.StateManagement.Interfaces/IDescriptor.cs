namespace DAA.StateManagement.Interfaces
{
    public interface IDescriptor
    {
        bool Intersects(IDescriptor descriptor);
    }
}
