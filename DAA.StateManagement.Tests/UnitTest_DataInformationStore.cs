using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;
using Moq.Protected;

using DAA.StateManagement.Interfaces;
using DAA.StateManagement.DataManagement;

namespace DAA.StateManagement.Tests
{
    [TestClass]
    public class UnitTest_DataInformationStore
    {
        private IEnumerable<ITerminalDescriptor> TerminalDescriptorsCollection { get => this.TerminalDescriptorsCollectionMock.Object; }
        private Mock<IEnumerable<ITerminalDescriptor>> TerminalDescriptorsCollectionMock { get; set; }

        private DataInformationStore<IDescriptor, object> TestInstance { get => this.TestInstanceMock.Object; }
        private Mock<DataInformationStore<IDescriptor, object>> TestInstanceMock { get; set; }
        private IProtectedMock<DataInformationStore<IDescriptor, object>> TestInstanceMockProtected { get => this.TestInstanceMock.Protected(); }


        [TestInitialize]
        public void BeforeEach()
        {
            this.TerminalDescriptorsCollectionMock = new Mock<IEnumerable<ITerminalDescriptor>>();

            this.TestInstanceMock = new Mock<DataInformationStore<IDescriptor, object>>();
            this.TestInstanceMock.CallBase = true;
        }


        [TestMethod]
        public void RetrieveDescriptors__ProxiesRetrieveKeys()
        {
            this.TestInstanceMockProtected
                .Setup<IEnumerable<IDescriptor>>("RetrieveKeys")
                .Returns(this.TerminalDescriptorsCollection);

            var result = this.TestInstance.RetrieveDescriptors();

            Assert.AreSame(this.TerminalDescriptorsCollection, result);
        }
    }
}
