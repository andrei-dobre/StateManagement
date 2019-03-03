using DAA.StateManagement.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DAA.StateManagement
{
    [TestClass]
    public class UnitTest_StateManagementEventsAggregator
    {
        private IDescriptor Descriptor => MockedDescriptor.Object;
        private Mock<IDescriptor> MockedDescriptor { get; set; }

        private INonTerminalDescriptor NonTerminalDescriptor => MockedNonTerminalDescriptor.Object;
        private Mock<INonTerminalDescriptor> MockedNonTerminalDescriptor { get; set; }

        private ITerminalDescriptor TerminalDescriptor => MockedTerminalDescriptor.Object;
        private Mock<ITerminalDescriptor> MockedTerminalDescriptor { get; set; }

        private IData Data => MockedData.Object;
        private Mock<IData> MockedData { get; set; }

        private StateManagementEventsAggregator<IData> TestInstance => MockedTestInstance.Object;
        private Mock<StateManagementEventsAggregator<IData>> MockedTestInstance { get; set; }


        [TestInitialize]
        public void BeforeEach()
        {
            MockedNonTerminalDescriptor = new Mock<INonTerminalDescriptor>();
            MockedTerminalDescriptor = new Mock<ITerminalDescriptor>();
            MockedDescriptor = new Mock<IDescriptor>();
            MockedData = new Mock<IData>();

            MockedTestInstance = new Mock<StateManagementEventsAggregator<IData>>();
            MockedTestInstance.CallBase = true;
        }


        [TestMethod]
        public void PublishDataChangedEvent__CorrectlyPublished()
        {
            var correctlyPublished = false;

            TestInstance.DataChangedEvent += (sender, args) =>
                correctlyPublished = sender == TestInstance && args == Descriptor;

            TestInstance.PublishDataChangedEvent(Descriptor);

            Assert.IsTrue(correctlyPublished);
        }

        [TestMethod]
        public void PublishDataChangedEvent_NoSubscribers_NoError()
        {
            var caught = false;

            try
            {
                TestInstance.PublishDataChangedEvent(Descriptor);
            }
            catch
            {
                caught = true;
            }

            Assert.IsFalse(caught);
        }

        [TestMethod]
        public void PublishCompositionChangedEvent__CorrectlyPublished()
        {
            var correctlyPublished = false;

            TestInstance.CompositionChangedEvent += (sender, args) =>
                correctlyPublished = sender == TestInstance && args == NonTerminalDescriptor;

            TestInstance.PublishCompositionChangedEvent(NonTerminalDescriptor);

            Assert.IsTrue(correctlyPublished);
        }

        [TestMethod]
        public void PublishCompositionChangedEvent_NoSubscribers_NoError()
        {
            var caught = false;

            try
            {
                TestInstance.PublishCompositionChangedEvent(NonTerminalDescriptor);
            }
            catch
            {
                caught = true;
            }

            Assert.IsFalse(caught);
        }

        [TestMethod]
        public void PublishInstanceChangedEvent__CorrectlyPublished()
        {
            var correctlyPublished = false;
            var givenArgs = new InstanceChangedEventArgs<IData>(TerminalDescriptor, Data);

            TestInstance.InstanceChangedEvent += (sender, args) =>
                correctlyPublished = sender == TestInstance && args == givenArgs;

            TestInstance.PublishInstanceChangedEvent(givenArgs);

            Assert.IsTrue(correctlyPublished);
        }

        [TestMethod]
        public void PublishInstanceChangedEvent_NoSubscribers_NoError()
        {
            var caught = false;
            var args = new InstanceChangedEventArgs<IData>(TerminalDescriptor, Data);

            try
            {
                TestInstance.PublishInstanceChangedEvent(args);
            }
            catch
            {
                caught = true;
            }

            Assert.IsFalse(caught);
        }
    }
}
