using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace DAA.StateManagement.Tests
{
    [TestClass]
    public class UnitTest_StateManagementServiceBuildingDirector
    {
        private Mock<IStateManagementServiceBuilder> BuilderMock { get; set; }

        private IStateManagementServiceBuilder Builder { get; set; }
        private StateManagementServiceBuildingDirector TestInstance { get; set; }


        [TestInitialize]
        public void BeforeEach()
        {
            this.BuilderMock = new Mock<IStateManagementServiceBuilder>();

            this.Builder = this.BuilderMock.Object;
            this.TestInstance = new StateManagementServiceBuildingDirector();
        }
        

        [TestMethod]
        public void Build__EventsAggregatorBuildFirst()
        {
            this.BuilderMock.Setup(_ => _.BuildEventsAggregator()).Verifiable();

            this.TestInstance.Build(this.Builder);

            this.BuilderMock.Verify();
        }

        [TestMethod]
        public void Build__DataQualitySupervisorBuildAfterEventsAggregator()
        {
            var callCounter = 0;
            var callOrderFollowed = false;

            this.BuilderMock.Setup(_ => _.BuildEventsAggregator()).Callback(() => ++callCounter);
            this.BuilderMock.Setup(_ => _.BuildDataQualitySupervisor()).Callback(() => callOrderFollowed = callCounter > 0).Verifiable();

            this.TestInstance.Build(this.Builder);

            this.BuilderMock.Verify();
            Assert.IsTrue(callOrderFollowed);
        }

        [TestMethod]
        public void Build__DataRetrieverAfterDataQualitySupervisorBuild()
        {
            var callCounter = 0;
            var callOrderFollowed = false;

            this.BuilderMock.Setup(_ => _.BuildDataQualitySupervisor()).Callback(() => ++callCounter);
            this.BuilderMock.Setup(_ => _.BuildDataRetriever()).Callback(() => callOrderFollowed = callCounter > 0).Verifiable();

            this.TestInstance.Build(this.Builder);

            this.BuilderMock.Verify();
            Assert.IsTrue(callOrderFollowed);
        }

        [TestMethod]
        public void Build__DataManipulatorAfterDataRetriever()
        {
            var callCounter = 0;
            var callOrderFollowed = false;

            this.BuilderMock.Setup(_ => _.BuildDataRetriever()).Callback(() => ++callCounter);
            this.BuilderMock.Setup(_ => _.BuildDataManipulator()).Callback(() => callOrderFollowed = callCounter > 0).Verifiable();

            this.TestInstance.Build(this.Builder);

            this.BuilderMock.Verify();
            Assert.IsTrue(callOrderFollowed);
        }

        [TestMethod]
        public void Build__DataPoolAfterDataManipulator()
        {
            var callCounter = 0;
            var callOrderFollowed = false;

            this.BuilderMock.Setup(_ => _.BuildDataManipulator()).Callback(() => ++callCounter);
            this.BuilderMock.Setup(_ => _.BuildDataPool()).Callback(() => callOrderFollowed = callCounter > 0).Verifiable();

            this.TestInstance.Build(this.Builder);

            this.BuilderMock.Verify();
            Assert.IsTrue(callOrderFollowed);
        }

        [TestMethod]
        public void Build__DataCollectionsManagerAfterDataPool()
        {
            var callCounter = 0;
            var callOrderFollowed = false;

            this.BuilderMock.Setup(_ => _.BuildDataPool()).Callback(() => ++callCounter);
            this.BuilderMock.Setup(_ => _.BuildDataCollectionsManager()).Callback(() => callOrderFollowed = callCounter > 0).Verifiable();

            this.TestInstance.Build(this.Builder);

            this.BuilderMock.Verify();
            Assert.IsTrue(callOrderFollowed);
        }
    }
}
