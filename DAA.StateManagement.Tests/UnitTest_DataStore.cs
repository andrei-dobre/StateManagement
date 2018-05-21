using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;
using Moq.Protected;

using DAA.Helpers;
using DAA.StateManagement.DataManagement;
using DAA.StateManagement.Interfaces;
using System;

namespace DAA.StateManagement.Tests
{
    [TestClass]
    public class UnitTest_DataStore
    {
        private IData Data { get => this.DataMock.Object; }
        private Mock<IData> DataMock { get; set; }

        private ITerminalDescriptor TerminalDescriptor { get => this.TerminalDescriptorMock.Object; }
        private Mock<ITerminalDescriptor> TerminalDescriptorMock { get; set; }
        
        private IEnumerable<ITerminalDescriptor> TerminalDescriptorsCollection { get => this.TerminalDescriptorsCollectionMock.Object; }
        private Mock<IEnumerable<ITerminalDescriptor>> TerminalDescriptorsCollectionMock { get; set; }

        private DataStore<IData> TestInstance { get => this.TestInstanceMock.Object; }
        private Mock<DataStore<IData>> TestInstanceMock { get; set; }
        private IProtectedMock<DataStore<IData>> TestInstanceMockProtected { get => this.TestInstanceMock.Protected(); }


        [TestInitialize]
        public void BeforeEach()
        {
            this.DataMock = new Mock<IData>();
            this.TerminalDescriptorMock = new Mock<ITerminalDescriptor>();
            this.TerminalDescriptorsCollectionMock = new Mock<IEnumerable<ITerminalDescriptor>>();

            this.TestInstanceMock = new Mock<DataStore<IData>>();
            this.TestInstanceMock.CallBase = true;
        }
    }
}
