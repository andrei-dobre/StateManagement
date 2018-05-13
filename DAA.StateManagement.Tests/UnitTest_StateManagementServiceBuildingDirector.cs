using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using DAA.StateManagement.Interfaces;

namespace DAA.StateManagement.Tests
{
    [TestClass]
    public class UnitTest_StateManagementServiceBuildingDirector
    {
        private Mock<IStateManagementServiceBuildingInterface> BuildingInterfaceMock { get; set; }

        private IStateManagementServiceBuildingInterface BuildingInterface { get => this.BuildingInterfaceMock.Object; }
        private StateManagementServiceBuildingDirector TestInstance { get; set; }


        [TestInitialize]
        public void BeforeEach()
        {
            this.BuildingInterfaceMock = new Mock<IStateManagementServiceBuildingInterface>();

            this.TestInstance = new StateManagementServiceBuildingDirector();
        }
        

        [TestMethod]
        public void Build__EventsAggregatorBuildFirst()
        {
            this.BuildingInterfaceMock.Setup(_ => _.BuildEventsAggregator()).Verifiable();

            this.TestInstance.Build(this.BuildingInterface);

            this.BuildingInterfaceMock.Verify();
        }

        [TestMethod]
        public void Build__DataQualitySupervisorBuildAfterEventsAggregator()
        {
            var callCounter = 0;
            var callOrderFollowed = false;

            this.BuildingInterfaceMock.Setup(_ => _.BuildEventsAggregator()).Callback(() => ++callCounter);
            this.BuildingInterfaceMock.Setup(_ => _.BuildDataQualitySupervisor()).Callback(() => callOrderFollowed = callCounter > 0).Verifiable();

            this.TestInstance.Build(this.BuildingInterface);

            this.BuildingInterfaceMock.Verify();
            Assert.IsTrue(callOrderFollowed);
        }

        [TestMethod]
        public void Build__DataRetrieverAfterDataQualitySupervisorBuild()
        {
            var callCounter = 0;
            var callOrderFollowed = false;

            this.BuildingInterfaceMock.Setup(_ => _.BuildDataQualitySupervisor()).Callback(() => ++callCounter);
            this.BuildingInterfaceMock.Setup(_ => _.BuildDataRetriever()).Callback(() => callOrderFollowed = callCounter > 0).Verifiable();

            this.TestInstance.Build(this.BuildingInterface);

            this.BuildingInterfaceMock.Verify();
            Assert.IsTrue(callOrderFollowed);
        }

        [TestMethod]
        public void Build__DataManipulatorAfterDataRetriever()
        {
            var callCounter = 0;
            var callOrderFollowed = false;

            this.BuildingInterfaceMock.Setup(_ => _.BuildDataRetriever()).Callback(() => ++callCounter);
            this.BuildingInterfaceMock.Setup(_ => _.BuildDataManipulator()).Callback(() => callOrderFollowed = callCounter > 0).Verifiable();

            this.TestInstance.Build(this.BuildingInterface);

            this.BuildingInterfaceMock.Verify();
            Assert.IsTrue(callOrderFollowed);
        }

        [TestMethod]
        public void Build__DataPoolAfterDataManipulator()
        {
            var callCounter = 0;
            var callOrderFollowed = false;

            this.BuildingInterfaceMock.Setup(_ => _.BuildDataManipulator()).Callback(() => ++callCounter);
            this.BuildingInterfaceMock.Setup(_ => _.BuildDataPool()).Callback(() => callOrderFollowed = callCounter > 0).Verifiable();

            this.TestInstance.Build(this.BuildingInterface);

            this.BuildingInterfaceMock.Verify();
            Assert.IsTrue(callOrderFollowed);
        }

        [TestMethod]
        public void Build__DataCollectionsManagerAfterDataPool()
        {
            var callCounter = 0;
            var callOrderFollowed = false;

            this.BuildingInterfaceMock.Setup(_ => _.BuildDataPool()).Callback(() => ++callCounter);
            this.BuildingInterfaceMock.Setup(_ => _.BuildDataCollectionsManager()).Callback(() => callOrderFollowed = callCounter > 0).Verifiable();

            this.TestInstance.Build(this.BuildingInterface);

            this.BuildingInterfaceMock.Verify();
            Assert.IsTrue(callOrderFollowed);
        }
    }
}
