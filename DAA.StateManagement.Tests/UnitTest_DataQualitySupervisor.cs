using System;
using System.Threading.Tasks;
using DAA.StateManagement.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DAA.StateManagement
{
    [TestClass]
    public class UnitTest_DataQualitySupervisor
    {
        private IDescriptor Descriptor => MockedDescriptor.Object;
        private Mock<IDescriptor> MockedDescriptor { get; set; }

        private IDataRefresher<IData> DataRefresher => MockedDataRefresher.Object;
        private Mock<IDataRefresher<IData>> MockedDataRefresher { get; set; }

        private DataQualitySupervisor<IData> TestInstance => MockedTestInstance.Object;
        private Mock<DataQualitySupervisor<IData>> MockedTestInstance { get; set; }


        [TestInitialize]
        public void BeforeEach()
        {
            MockedDescriptor = new Mock<IDescriptor>();
            MockedDataRefresher = new Mock<IDataRefresher<IData>>();

            MockedTestInstance = new Mock<DataQualitySupervisor<IData>>(DataRefresher);
            MockedTestInstance.CallBase = true;
        }


        [TestMethod]
        public async Task AcknowledgeStaleDataAsync__StaleDataRefreshed()
        {
            var awaited = false;

            MockedDataRefresher.Setup(_ => _.RefreshAsync(It.IsAny<IDescriptor>()))
                .Returns(Task.Delay(10).ContinueWith(_ => awaited = true));

            await TestInstance.AcknowledgeStaleDataAsync(Descriptor);

            MockedDataRefresher.Verify(_ => _.RefreshAsync(Descriptor));

            Assert.IsTrue(awaited);
        }
    }
}
