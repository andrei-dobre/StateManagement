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
        private Mock<TerminalDescriptorsFlyweightFactory> TestInstanceMock { get; set; }
        private IProtectedMock<TerminalDescriptorsFlyweightFactory> TestInstanceMockProtected { get => this.TestInstanceMock.Protected(); }

        private TerminalDescriptorsFlyweightFactory TestInstance { get => this.TestInstanceMock.Object; }


        [TestInitialize]
        public void BeforeEach()
        {
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
        public void Create__CollectionIterableMultipleTimes()
        {
            var result = this.TestInstance.Create(new object[] { new object() });
            var exceptionCaught = false;
            
            try
            {
                result.ForEach(_ => { });
                result.ForEach(_ => { });
            }
            catch
            {
                exceptionCaught = true;
            }

            Assert.IsFalse(exceptionCaught);
        }
    }
}
