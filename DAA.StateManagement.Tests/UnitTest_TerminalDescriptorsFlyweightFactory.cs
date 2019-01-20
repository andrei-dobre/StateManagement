using System.Collections.Generic;
using DAA.Helpers;
using DAA.StateManagement.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;

namespace DAA.StateManagement
{
    [TestClass]
    public class UnitTest_TerminalDescriptorsFlyweightFactory
    {
        private IData Data { get => DataMock.Object; }
        private Mock<IData> DataMock { get; set; }

        private TerminalDescriptorsFlyweightFactory TestInstance { get => TestInstanceMock.Object; }
        private Mock<TerminalDescriptorsFlyweightFactory> TestInstanceMock { get; set; }
        private IProtectedMock<TerminalDescriptorsFlyweightFactory> TestInstanceMockProtected { get => TestInstanceMock.Protected(); }


        [TestInitialize]
        public void BeforeEach()
        {
            DataMock = new Mock<IData>();

            TestInstanceMock = new Mock<TerminalDescriptorsFlyweightFactory>();
            TestInstanceMock.CallBase = true;
        }


        [TestMethod]
        public void Create_UnknownIntrinsicState_InstantiatedAndReturned()
        {
            var intrinsicState = new object();
            var terminalDescriptor = new Mock<ITerminalDescriptor>().Object;

            TestInstanceMockProtected
                .Setup<ITerminalDescriptor>("Instantiate", intrinsicState)
                .Returns(terminalDescriptor);

            var result = TestInstance.Create(intrinsicState);

            Assert.AreSame(terminalDescriptor, result);
        }

        [TestMethod]
        public void Create_KnownIntrinsicState_ReturnsExistingInstance()
        {
            var intrinsicState = new object();

            TestInstanceMockProtected
                .Setup<ITerminalDescriptor>("Instantiate", intrinsicState)
                .Returns(() => new Mock<ITerminalDescriptor>().Object);

            Assert.AreSame(TestInstance.Create(intrinsicState), TestInstance.Create(intrinsicState));
        }

        [TestMethod]
        public void Create_DifferentIntrinsicStates_DifferentInstancesForEachState()
        {
            var intrinsicStateOne = new object();
            var intrinsicStateTwo = new object();

            TestInstanceMockProtected
                .Setup<ITerminalDescriptor>("Instantiate", ItExpr.IsAny<object>())
                .Returns(() => new Mock<ITerminalDescriptor>().Object);

            Assert.AreSame(TestInstance.Create(intrinsicStateOne), TestInstance.Create(intrinsicStateOne));
            Assert.AreSame(TestInstance.Create(intrinsicStateTwo), TestInstance.Create(intrinsicStateTwo));
            Assert.IsFalse(ReferenceEquals(TestInstance.Create(intrinsicStateOne), TestInstance.Create(intrinsicStateTwo)));
        }

        [TestMethod]
        public void Create_IntrinsicStatesAutoBoxed_SameInstanceForEqualState()
        {
            var intrinsicState = RandomizationHelper.Instance.GetInt();

            TestInstanceMockProtected
                .Setup<ITerminalDescriptor>("Instantiate", intrinsicState)
                .Returns(() => new Mock<ITerminalDescriptor>().Object);

            Assert.AreSame(TestInstance.Create(intrinsicState), TestInstance.Create(intrinsicState));
        }


        [TestMethod]
        public void Create_Data_CreatedUsingDataIdentifier()
        {
            var descriptor = new Mock<ITerminalDescriptor>().Object;
            var dataIdentifier = new object();

            DataMock
                .Setup(_ => _.DataIdentifier)
                .Returns(dataIdentifier)
                .Verifiable();

            TestInstanceMock
                .Setup(_ => _.Create(dataIdentifier))
                .Returns(descriptor)
                .Verifiable();

            var result = TestInstance.Create(Data);

            DataMock.Verify();
            TestInstanceMock.Verify();

            Assert.AreSame(descriptor, result);
        }


        [TestMethod]
        public void Create_CollectionOfIntrinsicStates_TerminalDescriptorsCreatedForEach()
        {
            var intrinsicStates = new object[5];
            var terminalDescriptors = new List<ITerminalDescriptor>();

            for (var i = 0; i < intrinsicStates.Length; ++i)
            {
                terminalDescriptors.Add(new Mock<ITerminalDescriptor>().Object);
                intrinsicStates[i] = new object();

                TestInstanceMock
                    .Setup(_ => _.Create(intrinsicStates[i]))
                    .Returns(terminalDescriptors[i]);
            }

            var result = TestInstance.Create(intrinsicStates);

            Assert.IsTrue(terminalDescriptors.Equivalent(result));
        }


        [TestMethod]
        public void Create_CollectionOfData_TerminalDescriptorsCreatedForEach()
        {
            var dataCollection = new IData[5];
            var terminalDescriptors = new List<ITerminalDescriptor>();

            for (var i = 0; i < dataCollection.Length; ++i)
            {
                terminalDescriptors.Add(new Mock<ITerminalDescriptor>().Object);
                dataCollection[i] = new Mock<IData>().Object;

                TestInstanceMock
                    .Setup(_ => _.Create(dataCollection[i]))
                    .Returns(terminalDescriptors[i]);
            }

            var result = TestInstance.Create(dataCollection);

            Assert.IsTrue(terminalDescriptors.Equivalent(result));
        }
    }
}
