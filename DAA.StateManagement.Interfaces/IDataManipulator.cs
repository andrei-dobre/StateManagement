namespace DAA.StateManagement.Interfaces
{
    public interface IDataManipulator<TData>
        where TData : IData
    {
        void Update(TData target, TData source);
    }
}
