using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;
using Moq.Protected;

using DAA.Helpers;
using DAA.StateManagement.Interfaces;

namespace DAA.StateManagement.Tests
{
    [TestClass]
    public class UnitTest_TerminalDescriptorsFlyweightFactory : TerminalDescriptorsFlyweightFactory
    {
        private IData Data { get => this.DataMock.Object; }
        private Mock<IData> DataMock { get; set; }

        private TerminalDescriptorsFlyweightFactory TestInstance { get => this.TestInstanceMock.Object; }
        private Mock<TerminalDescriptorsFlyweightFactory> TestInstanceMock { get; set; }
        private IProtectedMock<TerminalDescriptorsFlyweightFactory> TestInstanceMockProtected { get => this.TestInstanceMock.Protected(); }


        [TestInitialize]
        public void BeforeEach()
        {
            this.DataMock = new Mock<IData>();

            this.TestInstanceMock = new Mock<TerminalDescriptorsFlyweightFactory>();
            this.TestInstanceMock.CallBase = true;
        }


        [TestMethod]
        public void Create_UnknownIntrinsicState_InstantiatedAndReturned()
        {
            var intrinsicState = new object();
            var terminalDescriptor = new Mock<ITerminalDescriptor>().Object;

            this.TestInstanceMockProtected
                .Setup<ITerminalDescriptor>(nameof(Instantiate), intrinsicState)
                .Returns(terminalDescriptor);

            var result = this.TestInstance.Create(intrinsicState);

            Assert.IsTrue(Object.ReferenceEquals(terminalDescriptor, result));
        }

        [TestMethod]
        public void Create_KnownIntrinsicState_ReturnsExistingInstance()
        {
            var intrinsicState = new object();

            this.TestInstanceMockProtected
                .Setup<ITerminalDescriptor>(nameof(Instantiate), intrinsicState)
                .Returns(() => new Mock<ITerminalDescriptor>().Object);

            Assert.IsTrue(Object.ReferenceEquals(this.TestInstance.Create(intrinsicState), this.TestInstance.Create(intrinsicState)));
        }

        [TestMethod]
        public void Create_DifferentIntrinsicStates_DifferentInstancesForEachState()
        {
            var intrinsicStateOne = new object();
            var intrinsicStateTwo = new object();

            this.TestInstanceMockProtected
                .Setup<ITerminalDescriptor>(nameof(Instantiate), ItExpr.IsAny<object>())
                .Returns(() => new Mock<ITerminalDescriptor>().Object);

            Assert.IsTrue(Object.ReferenceEquals(this.TestInstance.Create(intrinsicStateOne), this.TestInstance.Create(intrinsicStateOne)));
            Assert.IsTrue(Object.ReferenceEquals(this.TestInstance.Create(intrinsicStateTwo), this.TestInstance.Create(intrinsicStateTwo)));
            Assert.IsFalse(Object.ReferenceEquals(this.TestInstance.Create(intrinsicStateOne), this.TestInstance.Create(intrinsicStateTwo)));
        }


        [TestMethod]
        public void Create_Data_CreatedUsingDataIdentifier()
        {
            var descriptor = new Mock<ITerminalDescriptor>().Object;
            var dataIdentifier = new object();

            this.DataMock
                .Setup(_ => _.DataIdentifier)
                .Returns(dataIdentifier)
                .Verifiable();

            this.TestInstanceMock
                .Setup(_ => _.Create(dataIdentifier))
                .Returns(descriptor)
                .Verifiable();

            var result = this.TestInstance.Create(this.Data);

            this.DataMock.Verify();
            this.TestInstanceMock.Verify();

            Assert.AreSame(descriptor, result);
        }


        [TestMethod]
        public void Create_CollectionOfInstrinsicStates_TerminalDescriptorsCreatedForEach()
        {
            var intrinsicStates = new object[5];
            var terminalDescriptors = new List<ITerminalDescriptor>();

            for (int i = 0; i < intrinsicStates.Length; ++i)
            {
                terminalDescriptors.Add(new Mock<ITerminalDescriptor>().Object);
                intrinsicStates[i] = new object();

                this.TestInstanceMock
                    .Setup(_ => _.Create(intrinsicStates[i]))
                    .Returns(terminalDescriptors[i]);
            }

            var result = this.TestInstance.Create(intrinsicStates);

            Assert.IsTrue(terminalDescriptors.Equivalent(result));
        }


        [TestMethod]
        public void Create_CollectionOfData_TerminalDescriptorsCreatedForEach()
        {
            var dataCollection = new IData[5];
            var terminalDescriptors = new List<ITerminalDescriptor>();

            for (int i = 0; i < dataCollection.Length; ++i)
            {
                terminalDescriptors.Add(new Mock<ITerminalDescriptor>().Object);
                dataCollection[i] = new Mock<IData>().Object;

                this.TestInstanceMock
                    .Setup(_ => _.Create(dataCollection[i]))
                    .Returns(terminalDescriptors[i]);
            }

            var result = this.TestInstance.Create(dataCollection);

            Assert.IsTrue(terminalDescriptors.Equivalent(result));
        }
    }
}
