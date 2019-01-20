using DAA.StateManagement.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DAA.StateManagement
{
    [TestClass]
    public class UnitTest_StateEventsAggregator
    {
        private IDescriptor Descriptor => MockedDescriptor.Object;
        private Mock<IDescriptor> MockedDescriptor { get; set; }

        private INonTerminalDescriptor NonTerminalDescriptor => MockedNonTerminalDescriptor.Object;
        private Mock<INonTerminalDescriptor> MockedNonTerminalDescriptor { get; set; }

        private StateEventsAggregator<IData> TestInstance => MockedTestInstance.Object;
        private Mock<StateEventsAggregator<IData>> MockedTestInstance { get; set; }


        [TestInitialize]
        public void BeforeEach()
        {
            MockedNonTerminalDescriptor = new Mock<INonTerminalDescriptor>();
            MockedDescriptor = new Mock<IDescriptor>();

            MockedTestInstance = new Mock<StateEventsAggregator<IData>>();
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
    }
}
