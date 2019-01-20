using System.Collections.Generic;
using DAA.StateManagement.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DAA.StateManagement.Stores
{
    [TestClass]
    public class UnitTest_StateManagementStore
    {
        private IEnumerable<ITerminalDescriptor> TerminalDescriptorsCollection { get => TerminalDescriptorsCollectionMock.Object; }
        private Mock<IEnumerable<ITerminalDescriptor>> TerminalDescriptorsCollectionMock { get; set; }

        private StateManagementStore<IDescriptor, object> TestInstance { get => TestInstanceMock.Object; }
        private Mock<StateManagementStore<IDescriptor, object>> TestInstanceMock { get; set; }


        [TestInitialize]
        public void BeforeEach()
        {
            TerminalDescriptorsCollectionMock = new Mock<IEnumerable<ITerminalDescriptor>>();

            TestInstanceMock = new Mock<StateManagementStore<IDescriptor, object>>();
            TestInstanceMock.CallBase = true;
        }


        [TestMethod]
        public void RetrieveDescriptors__ProxiesRetrieveKeys()
        {
            TestInstanceMock
                .Setup<IEnumerable<IDescriptor>>(_ => _.RetrieveKeys())
                .Returns(TerminalDescriptorsCollection);

            var result = TestInstance.RetrieveDescriptors();

            Assert.AreSame(TerminalDescriptorsCollection, result);
        }
    }
}
