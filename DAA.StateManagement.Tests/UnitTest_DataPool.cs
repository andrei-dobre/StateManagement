using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;
using Moq.Protected;

namespace DAA.StateManagement.Tests
{
    [TestClass]
    public class UnitTest_DataPool : DataPool<IData>
    {
        private Mock<IDescriptor> DescriptorMock { get; set; }
        private Mock<ICollection<IData>> DataCollectionMock { get; set; }
        private Mock<ICollection<ITerminalDescriptor>> TerminalDescriptorsCollectionMock { get; set; }
        private Mock<DataPool<IData>> TestInstanceMock { get; set; }
        private IProtectedMock<DataPool<IData>> TestInstanceMockProtected { get; set; }

        private IDescriptor Descriptor { get; set; }
        private ICollection<IData> DataCollection { get; set; }
        private ICollection<ITerminalDescriptor> TerminalDescriptorsCollection { get; set; }
        private DataPool<IData> TestInstance { get; set; }


        [TestInitialize]
        public void BeforeEach()
        {
            this.DescriptorMock = new Mock<IDescriptor>();
            this.DataCollectionMock = new Mock<ICollection<IData>>();
            this.TerminalDescriptorsCollectionMock = new Mock<ICollection<ITerminalDescriptor>>();

            this.TestInstanceMock = new Mock<DataPool<IData>>();
            this.TestInstanceMockProtected = this.TestInstanceMock.Protected();

            this.Descriptor = this.DescriptorMock.Object;
            this.DataCollection = this.DataCollectionMock.Object;
            this.TerminalDescriptorsCollection = this.TerminalDescriptorsCollectionMock.Object;
            this.TestInstance = this.TestInstanceMock.Object;
        }


        [TestMethod]
        public void Store__NonterminalDescriptorRegisteredIfUnknown()
        {
            this.TestInstanceMockProtected.Setup(nameof(RegisterDescriptorIfNonterminalAndUnknown), ItExpr.IsAny<IDescriptor>());
            this.TestInstanceMockProtected.Setup(nameof(StoreDataAndProvideTerminalDescriptors), ItExpr.IsAny<ICollection<IData>>());
            this.TestInstanceMockProtected.Setup(nameof(UpdateCompositionOfDescriptorIfNonterminal), ItExpr.IsAny<IDescriptor>(), ItExpr.IsAny<ICollection<ITerminalDescriptor>>());

            this.TestInstance.Store(this.Descriptor, this.DataCollection);

            this.TestInstanceMockProtected.Verify(nameof(RegisterDescriptorIfNonterminalAndUnknown), Times.Once(), this.Descriptor);
        }

        [TestMethod]
        public void Store__DataStored()
        {
            this.TestInstanceMockProtected.Setup(nameof(RegisterDescriptorIfNonterminalAndUnknown), ItExpr.IsAny<IDescriptor>());
            this.TestInstanceMockProtected.Setup(nameof(StoreDataAndProvideTerminalDescriptors), ItExpr.IsAny<ICollection<IData>>());
            this.TestInstanceMockProtected.Setup(nameof(UpdateCompositionOfDescriptorIfNonterminal), ItExpr.IsAny<IDescriptor>(), ItExpr.IsAny<ICollection<ITerminalDescriptor>>());

            this.TestInstance.Store(this.Descriptor, this.DataCollection);

            this.TestInstanceMockProtected.Verify(nameof(StoreDataAndProvideTerminalDescriptors), Times.Once(), this.DataCollection);
        }

        [TestMethod]
        public void Store__CompositionOfNonterminalDescriptorUpdated()
        {
            this.TestInstanceMockProtected.Setup(nameof(RegisterDescriptorIfNonterminalAndUnknown), ItExpr.IsAny<IDescriptor>());
            this.TestInstanceMockProtected.Setup(nameof(UpdateCompositionOfDescriptorIfNonterminal), ItExpr.IsAny<IDescriptor>(), ItExpr.IsAny<ICollection<ITerminalDescriptor>>());
            this.TestInstanceMockProtected
                .Setup<ICollection<ITerminalDescriptor>>(nameof(StoreDataAndProvideTerminalDescriptors), ItExpr.IsAny<ICollection<IData>>())
                .Returns(() => this.TerminalDescriptorsCollection);

            this.TestInstance.Store(this.Descriptor, this.DataCollection);

            this.TestInstanceMockProtected.Verify(nameof(UpdateCompositionOfDescriptorIfNonterminal), Times.Once(), this.Descriptor, this.TerminalDescriptorsCollection);
        }
    }
}
