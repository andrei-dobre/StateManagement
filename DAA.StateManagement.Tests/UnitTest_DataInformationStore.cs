using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;
using DAA.StateManagement.Interfaces;
using DAA.StateManagement.DataManagement;

namespace DAA.StateManagement.Tests
{
    [TestClass]
    public class UnitTest_DataInformationStore
    {
        private IEnumerable<ITerminalDescriptor> TerminalDescriptorsCollection { get => TerminalDescriptorsCollectionMock.Object; }
        private Mock<IEnumerable<ITerminalDescriptor>> TerminalDescriptorsCollectionMock { get; set; }

        private DataInformationStore<IDescriptor, object> TestInstance { get => TestInstanceMock.Object; }
        private Mock<DataInformationStore<IDescriptor, object>> TestInstanceMock { get; set; }


        [TestInitialize]
        public void BeforeEach()
        {
            TerminalDescriptorsCollectionMock = new Mock<IEnumerable<ITerminalDescriptor>>();

            TestInstanceMock = new Mock<DataInformationStore<IDescriptor, object>>();
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
