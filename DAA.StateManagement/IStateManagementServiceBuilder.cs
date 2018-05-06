namespace DAA.StateManagement
{
    public interface IStateManagementServiceBuilder
    {
        void BuildDataCollectionsManager();

        void BuildDataManipulator();

        void BuildDataPool();

        void BuildDataQualitySupervisor();

        void BuildDataRetriever();

        void BuildEventsAggregator();

        void BuildTerminalDescriptorsFlyweightFactory();

        StateManagementService ExtractResult();
    }
}
