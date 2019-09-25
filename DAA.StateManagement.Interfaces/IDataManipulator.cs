namespace DAA.StateManagement.Interfaces
{
    public interface IDataManipulator<in TData>
        where TData : IData
    {
        void Update(TData target, TData source);
    }
}
