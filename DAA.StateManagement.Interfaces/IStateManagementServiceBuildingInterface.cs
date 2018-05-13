namespace DAA.StateManagement.Interfaces
{
    public interface IStateManagementServiceBuildingInterface
    {
        void BuildDataCollectionsManager();

        void BuildDataManipulator();

        void BuildDataPool();

        void BuildDataQualitySupervisor();

        void BuildDataRetriever();

        void BuildEventsAggregator();

        void BuildTerminalDescriptorsFlyweightFactory();
    }
}
